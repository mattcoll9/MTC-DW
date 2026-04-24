# MTC Data Warehouse — Application Specification

## Purpose

MTC-DW is a single-instance Windows desktop application that pulls data from external business systems into an on-premises SQL Server 2019 database for cross-platform reporting and analysis. The initial data source is Deputy (rostering/timesheets). Additional sources (YourPayroll/KeyPay, RevSport, other SQL Servers, spreadsheets) can be added as modules in future.

---

## Architecture

```
┌──────────────────────────────────────┐
│  MTC-DW.exe  (.NET Framework 4.8)    │
│  WinForms desktop application        │
│                                      │
│  Forms/       — UI panels & forms    │
│  Services/    — DB, API, Scheduler   │
│  Models/      — Plain data classes   │
└──────────────┬───────────────────────┘
               │ ADO.NET SqlClient
               ▼
┌──────────────────────────────────────┐
│  SQL Server 2019                     │
│  Database: MTC_DataWarehouse         │
│                                      │
│  dbo schema    — control tables      │
│  deputy schema — Deputy data         │
│  (future schemas per source)         │
└──────────────────────────────────────┘
               │ HTTP/JSON
               ▼
┌──────────────────────────────────────┐
│  Deputy API v1  (au.deputy.com)      │
│  (future: YourPayroll, RevSport...)  │
└──────────────────────────────────────┘
```

---

## Database Schema

### dbo.Config — key/value settings store

| Column | Type | Description |
|--------|------|-------------|
| ConfigKey | VARCHAR(100) PK | Setting name, e.g. `Deputy.OAuthToken` |
| ConfigValue | NVARCHAR(MAX) | Setting value |
| UpdatedAt | DATETIME2 | Last changed |

**Built-in keys:**

| Key | Default | Purpose |
|-----|---------|---------|
| `Deputy.BaseUrl` | `https://YOURINSTANCE.au.deputy.com/api/v1/` | Deputy API base URL |
| `Deputy.OAuthToken` | *(empty)* | Deputy OAuth token (no "OAuth " prefix) |
| `Deputy.SyncDaysBack` | `90` | How many calendar days of timesheets to pull |

---

### dbo.Jobs — scheduled job definitions

| Column | Type | Description |
|--------|------|-------------|
| Id | INT IDENTITY PK | |
| JobName | VARCHAR(100) | Display name |
| SourceType | VARCHAR(50) | `Deputy` (extensible) |
| EntityType | VARCHAR(50) | `Timesheets`, `Employees`, `OperationalUnits`, `WorkTypes` |
| ScheduleType | VARCHAR(20) | `Once` or `Recurring` |
| IntervalMinutes | INT NULL | Interval for Recurring jobs (NULL for Once) |
| NextRunTime | DATETIME2 NULL | When the job should next fire |
| LastRunTime | DATETIME2 NULL | When it last ran |
| IsEnabled | BIT | 1 = active in scheduler |

---

### dbo.JobHistory — run log

| Column | Type | Description |
|--------|------|-------------|
| Id | INT IDENTITY PK | |
| JobId | INT FK | References dbo.Jobs |
| StartedAt | DATETIME2 | UTC-local start time |
| CompletedAt | DATETIME2 NULL | NULL while running |
| Status | VARCHAR(20) | `Running`, `Success`, `Failed` |
| RecordsAffected | INT NULL | Rows upserted |
| ErrorMessage | NVARCHAR(MAX) NULL | Exception message on failure |

---

### deputy.Timesheets

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT PK | Deputy timesheet ID |
| TimesheetDate | DATE | Shift date |
| StartTime | DATETIME2 | Shift start |
| EndTime | DATETIME2 | Shift end |
| TotalMinutes | DECIMAL(10,2) | Duration in minutes (API returns hours × 60) |
| Cost | DECIMAL(18,2) | Labour cost |
| IsApproved | BIT | Approval status |
| EmployeeId | BIGINT | FK → deputy.Employees |
| OperationalUnitId | BIGINT | FK → deputy.OperationalUnits |
| WorkTypeId | BIGINT | FK → deputy.WorkTypes |
| SyncedAt | DATETIME2 | Last synced |

---

### deputy.Employees

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT PK | Deputy employee ID |
| DisplayName | NVARCHAR(200) | Full display name *(see PII note)* |
| RoleTitle | NVARCHAR(200) | Position/role title |
| YearOfBirth | SMALLINT NULL | Year only — age demographic |
| StartYear | SMALLINT NULL | Employment start year — tenure |
| IsActive | BIT | Current active status |
| SyncedAt | DATETIME2 | Last synced |

**PII Note:** `DisplayName` is stored at the operator's explicit request for internal single-instance use. No email, phone, address, or full date of birth is stored. `YearOfBirth` is retained for age band demographic analysis only.

---

### deputy.OperationalUnits

| Column | Type | Notes |
|--------|------|-------|
| Id | BIGINT PK | |
| Code | VARCHAR(50) | Area export code |
| UnitName | NVARCHAR(200) | Department/location name |
| IsActive | BIT | |
| SyncedAt | DATETIME2 | |

---

### deputy.WorkTypes

| Column | Type | Notes |
|--------|------|-------|
| Id | BIGINT PK | |
| Code | VARCHAR(50) | Export code |
| WorkTypeName | NVARCHAR(200) | Description |
| IsActive | BIT | |
| SyncedAt | DATETIME2 | |

---

## Deputy API Integration

- **Base URL:** configured in `dbo.Config` → `Deputy.BaseUrl`
- **Authentication:** `Authorization: OAuth {token}` header on every request
- **Protocol:** HTTP GET for resource lists, HTTP POST for QUERY endpoints
- **Pagination:** `max=100&start=N` query params (GET) or JSON body fields (POST)
- **Upsert strategy:** SQL MERGE on primary key — safe to re-run at any time

### Endpoints used

| Entity | Method | Endpoint |
|--------|--------|---------|
| Operational Units | GET | `resource/OperationalUnit?max=100&start=N` |
| Work Types | GET | `resource/WorkType?max=100&start=N` |
| Employees | GET | `resource/Employee?max=100&start=N` |
| Timesheets | POST | `resource/Timesheet/QUERY` with `{"start":N,"max":100}` body |

### Sync order

Timesheets are synced last so FK lookups to Employees/Units/WorkTypes resolve correctly. All syncs use MERGE so partial re-runs leave no duplicates.

---

## Scheduler

- In-process `System.Threading.Timer` ticks every **60 seconds**
- Queries `dbo.Jobs WHERE IsEnabled=1 AND NextRunTime <= GETDATE()`
- Fires due jobs sequentially on a background thread (SyncLock prevents overlap)
- After a Recurring job completes: `NextRunTime += IntervalMinutes`
- After a Once job completes: `NextRunTime` is set to NULL (won't re-fire)
- All outcomes written to `dbo.JobHistory`
- The app **must remain open** for scheduled jobs to fire

### Available intervals (UI)

15 min / 1 hour / 4 hours / 12 hours / 24 hours (or any integer entered manually)

---

## Historical Backfill

A **Backfill** job type exists specifically for one-time historical data loads (e.g. importing 2 years of Deputy timesheets).

### How it works

| Step | What happens |
|------|-------------|
| 1 | Operator creates a Backfill job with a date range (e.g. 01/01/2023 → today) and a chunk size (default 30 days) |
| 2 | Scheduler fires the job at the configured interval (default 2 min between chunks) |
| 3 | Each run pulls **one chunk** (e.g. Jan 2023) via the date-filtered Deputy API query |
| 4 | `SyncCursor` in `dbo.Jobs` advances to the next chunk start |
| 5 | When `SyncCursor > SyncToDate`, the job auto-disables itself |

### Resilience

- `SyncCursor` is written to the database **after each successful chunk**, not at the end. If the app is closed mid-backfill, it resumes from the last completed chunk on next restart — no data is skipped or doubled.
- All inserts use MERGE (upsert), so re-running a chunk that partially succeeded is safe.

### Jobs grid progress indicator

The **Schedule** column in the Jobs grid shows `Backfill XX%` — calculated from `(cursor − fromDate) / (toDate − fromDate)`.

### Typical 2-year backfill setup

| Field | Value |
|-------|-------|
| Schedule | Backfill |
| Entity | Timesheets |
| Pull From | 01/01/2023 |
| To | today |
| Chunk (days) | 30 |
| Interval (min) | 2 |
| First Run | now |

Expected runtime: 24 monthly chunks × ~60 sec per chunk ≈ ~25 minutes total, running unattended while the app is open.

### dbo.Jobs backfill columns

| Column | Type | Description |
|--------|------|-------------|
| SyncFromDate | DATE | Start of the full date range to load |
| SyncToDate | DATE | End of the full date range to load |
| ChunkDays | INT | Days per API pull (default 30) |
| SyncCursor | DATE | Next chunk start; auto-managed by scheduler |

---

## UI Panels

| Panel | Purpose |
|-------|---------|
| Dashboard | Row-count stats (4 boxes) + last 20 job runs |
| Jobs | Job CRUD grid + per-job history; "Run Now" button |
| Deputy | Tabbed read-only grids: Timesheets (date filter), Employees, Op Units, Work Types |
| Settings | SQL connection string editor + API config key/value store |
| Logs | Filterable run history (date range + status) |

---

## Adding a Future Data Source

1. Create a new SQL schema (e.g. `CREATE SCHEMA revsport`)
2. Create tables inside that schema
3. Add a new `XxxApiService.vb` and `XxxSyncService.vb` in `Services/`
4. Add the new `SourceType` string (e.g. `"RevSport"`) to `SchedulerService.DispatchJob`
5. Add a new panel in `Forms/` if a dedicated view is needed
6. Wire the panel into `MainForm.ShowPanel`

No changes to any existing services or tables are required.
