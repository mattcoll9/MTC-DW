# MTC Data Warehouse — Application Specification

## Purpose

MTC-DW is a single-instance Windows desktop application that pulls data from external business systems into an on-premises SQL Server 2019 database for cross-platform reporting and analysis. Current data sources: **Deputy** (rostering/timesheets, REST JSON API) and **RevSport** (members/events, CSV export via web session). Additional sources (YourPayroll/KeyPay, other SQL Servers, spreadsheets) can be added as modules in future.

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
│  dbo schema      — control tables   │
│  deputy schema   — Deputy data      │
│  revsport schema — RevSport data    │
└──────────────────────────────────────┘
         │ HTTP/JSON              │ HTTP/CSV
         ▼                        ▼
┌─────────────────┐   ┌──────────────────────────┐
│  Deputy API v1  │   │  RevSport portal          │
│  (au.deputy.com)│   │  (portal.revolutionise    │
│                 │   │   .com.au)                │
└─────────────────┘   └──────────────────────────┘
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
| `RevSport.Email` | *(empty)* | RevSport portal login email |
| `RevSport.Password` | *(empty)* | RevSport portal login password |
| `RevSport.TotpSeed` | *(empty)* | Base-32 TOTP seed (leave empty if 2FA not enabled on the account) |
| `RevSport.SeasonId` | `43649` | Season ID for member sync (43649 = 2025-26) |
| `RevSport.EventsDaysBack` | `90` | Lookback window (days) for events/attendees when no date range is set on the job |

---

### dbo.Jobs — scheduled job definitions

| Column | Type | Description |
|--------|------|-------------|
| Id | INT IDENTITY PK | |
| JobName | VARCHAR(100) | Display name |
| SourceType | VARCHAR(50) | `Deputy` or `RevSport` |
| EntityType | VARCHAR(50) | Deputy: `Timesheets`, `Rosters`, `Employees`, `OperationalUnits`, `Company`, `Departments`; RevSport: `Members`, `Events`, `EventAttendees` |
| ScheduleType | VARCHAR(20) | `Once`, `Recurring`, or `Backfill` |
| IntervalMinutes | INT NULL | Interval for Recurring/Backfill jobs |
| NextRunTime | DATETIME2 NULL | When the job should next fire |
| LastRunTime | DATETIME2 NULL | When it last ran |
| IsEnabled | BIT | 1 = active in scheduler |
| SyncFromDate | DATE NULL | Start of backfill/event date range |
| SyncToDate | DATE NULL | End of backfill/event date range |
| ChunkDays | INT NULL | Days per API pull for backfill jobs |
| SyncCursor | DATE NULL | Backfill progress cursor; auto-managed by scheduler |

---

### dbo.JobHistory — run log

| Column | Type | Description |
|--------|------|-------------|
| Id | INT IDENTITY PK | |
| JobId | INT FK | References dbo.Jobs |
| StartedAt | DATETIME2 | UTC-local start time |
| CompletedAt | DATETIME2 NULL | NULL while running |
| Status | VARCHAR(20) | `Running`, `Success`, `Failed` |
| RecordsAffected | INT NULL | Rows upserted / inserted |
| ErrorMessage | NVARCHAR(MAX) NULL | Exception message on failure |

---

### deputy.Timesheets

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT PK | Deputy timesheet ID |
| EmployeeId | BIGINT NULL | FK → deputy.Employees |
| OperationalUnitId | BIGINT NULL | FK → deputy.OperationalUnits |
| RosterId | BIGINT NULL | FK → deputy.Rosters |
| TimesheetDate | DATE NULL | Shift date |
| StartTime | DATETIME2 NULL | Shift start (localised) |
| EndTime | DATETIME2 NULL | Shift end (localised) |
| MealbreakMinutes | DECIMAL(5,2) NULL | Break duration in minutes |
| TotalHours | DECIMAL(6,2) NULL | Paid hours |
| TotalHoursInv | DECIMAL(6,2) NULL | Invoice hours |
| Cost | DECIMAL(18,4) NULL | Labour cost |
| OnCost | DECIMAL(18,4) NULL | On-cost |
| IsApproved | BIT | Approval status |
| IsPayRuleApproved | BIT | Pay rule approval |
| IsLeave | BIT | Leave flag |
| IsInProgress | BIT | In-progress flag |
| Discarded | BIT | Discarded flag |
| ReviewState | INT NULL | Review state code |
| Modified | DATETIME2 NULL | Last modified in Deputy |
| SyncedAt | DATETIME2 | Last synced |

---

### deputy.Rosters

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT PK | Deputy roster ID |
| EmployeeId | BIGINT NULL | FK → deputy.Employees |
| OperationalUnitId | BIGINT NULL | FK → deputy.OperationalUnits |
| TimesheetId | BIGINT NULL | Matched timesheet |
| RosterDate | DATE NULL | Roster date |
| StartTime | DATETIME2 NULL | Scheduled start |
| EndTime | DATETIME2 NULL | Scheduled end |
| MealbreakMinutes | DECIMAL(5,2) NULL | Break duration |
| TotalHours | DECIMAL(6,2) NULL | Scheduled hours |
| Cost | DECIMAL(18,4) NULL | Scheduled cost |
| OnCost | DECIMAL(18,4) NULL | On-cost |
| Published | BIT | Published flag |
| IsOpen | BIT | Open shift flag |
| Modified | DATETIME2 NULL | Last modified in Deputy |
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
| CompanyId | BIGINT NULL | FK → deputy.Company |
| IsActive | BIT | Current active status |
| SyncedAt | DATETIME2 | Last synced |

**PII Note:** `DisplayName` is stored at the operator's explicit request for internal single-instance use. No email, phone, address, or full date of birth is stored. `YearOfBirth` is retained for age band demographic analysis only.

---

### deputy.OperationalUnits

| Column | Type | Notes |
|--------|------|-------|
| Id | BIGINT PK | |
| PayrollExportName | VARCHAR(50) | Area export code (no `Code` field in API) |
| UnitName | NVARCHAR(200) | Department/location name |
| CompanyId | BIGINT NULL | FK → deputy.Company |
| CompanyCode | VARCHAR(50) NULL | Denormalised from Company |
| CompanyName | NVARCHAR(200) NULL | Denormalised from Company |
| IsActive | BIT | |
| SyncedAt | DATETIME2 | |

---

### deputy.Company

| Column | Type | Notes |
|--------|------|-------|
| Id | BIGINT PK | |
| CompanyName | NVARCHAR(200) | |
| Code | VARCHAR(50) NULL | |
| IsActive | BIT | |
| SyncedAt | DATETIME2 | |

---

### deputy.Departments

| Column | Type | Notes |
|--------|------|-------|
| Id | BIGINT PK | |
| DepartmentName | NVARCHAR(200) | |
| CompanyId | BIGINT NULL | FK → deputy.Company |
| IsActive | BIT | |
| SyncedAt | DATETIME2 | |

---

### revsport.Members

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY PK | |
| SeasonId | INT | Season identifier (e.g. 43649 = 2025-26) |
| ParentBodyId | NVARCHAR(50) | Member ID in RevSport (unique per season) |
| FullName | NVARCHAR(200) NULL | |
| DateOfBirth | DATE NULL | |
| Gender | NVARCHAR(20) NULL | |
| CreationTime | DATETIME NULL | Registration date |
| Address | NVARCHAR(500) NULL | |
| PhoneHome | NVARCHAR(50) NULL | |
| PhoneMobile | NVARCHAR(50) NULL | |
| Email | NVARCHAR(200) NULL | |
| PaymentStatus | NVARCHAR(50) NULL | |
| PaymentDate | DATE NULL | |
| PaymentMethod | NVARCHAR(100) NULL | |
| PaymentReceipt | NVARCHAR(100) NULL | |
| PaymentWho | NVARCHAR(200) NULL | |
| Deceased | BIT NULL | |
| LastUpdated | DATETIME NULL | Last modified in RevSport |
| SyncedAt | DATETIME | Last synced |

Unique index on `(SeasonId, ParentBodyId)`. Sync strategy: DELETE season + bulk insert.

---

### revsport.Events

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY PK | |
| DateStart | DATE | Report date range start (sync window) |
| DateEnd | DATE | Report date range end (sync window) |
| EventName | NVARCHAR(500) NULL | |
| EventDate | DATE NULL | |
| Category | NVARCHAR(200) NULL | |
| StartTime | NVARCHAR(50) NULL | Text as returned in CSV |
| EndTime | NVARCHAR(50) NULL | Text as returned in CSV |
| Registered | INT NULL | Total registrations |
| Attended | INT NULL | Total attendees |
| Revenue | DECIMAL(10,2) NULL | |
| SyncedAt | DATETIME | Last synced |

Sync strategy: DELETE (DateStart, DateEnd) range + bulk insert.

---

### revsport.EventAttendees

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY PK | |
| DateStart | DATE | Report date range start (sync window) |
| DateEnd | DATE | Report date range end (sync window) |
| MemberId | NVARCHAR(50) NULL | RevSport ParentBodyId |
| MemberName | NVARCHAR(200) NULL | |
| Email | NVARCHAR(200) NULL | |
| EventName | NVARCHAR(500) NULL | |
| EventDate | DATE NULL | |
| Category | NVARCHAR(200) NULL | |
| AttendanceStatus | NVARCHAR(50) NULL | |
| AmountPaid | DECIMAL(10,2) NULL | |
| SyncedAt | DATETIME | Last synced |

Sync strategy: DELETE (DateStart, DateEnd) range + bulk insert.

---

## Deputy API Integration

- **Base URL:** configured in `dbo.Config` → `Deputy.BaseUrl`
- **Authentication:** `Authorization: OAuth {token}` header on every request
- **Protocol:** HTTP GET for resource lists, HTTP POST for QUERY endpoints
- **Pagination:** `max=500&start=N` query params (GET) or JSON body fields (POST); chunk size 500
- **Upsert strategy:** SQL MERGE on primary key — safe to re-run at any time

### Endpoints used

| Entity | Method | Endpoint |
|--------|--------|---------|
| Timesheets | POST | `resource/Timesheet/QUERY` with date range + paging |
| Rosters | POST | `resource/Roster/QUERY` with date range + paging |
| Employees | POST | `resource/Employee/QUERY` with `{"search":{}}` |
| OperationalUnits | POST | `resource/OperationalUnit/QUERY` |
| Company | POST | `resource/Company/QUERY` |
| Departments | POST | `resource/Department/QUERY` |

### Sync order

Employees and OperationalUnits should be synced before Timesheets/Rosters so FK lookups resolve correctly. All syncs use MERGE so partial re-runs leave no duplicates.

---

## RevSport Integration

RevSport does not expose a JSON API. Reports are downloaded as CSV from the web portal via a browser-like session.

### Authentication flow

1. `GET /bsyc/login` — server sets `XSRF-TOKEN` cookie
2. `POST /bsyc/login` with credentials + CSRF token
3. If 2FA required (response HTML contains `otpModal`): `POST /bsyc/tfa` with a TOTP code generated from `RevSport.TotpSeed`

Session is maintained in a `CookieContainer`. If a download returns HTML instead of CSV (session expiry), the service re-authenticates once and retries automatically.

### Report URLs

| Report | URL pattern |
|--------|-------------|
| Members | `GET /bsyc/members/reports/download?season_id=N&fields[]=...&report-format=csv` |
| Events (per-event) | `GET /bsyc/events/reports/attendance?report_type=per-event&dateStart=DD/MM/YYYY&dateEnd=DD/MM/YYYY&report-format=csv` |
| Event Attendees (per-member) | `GET /bsyc/events/reports/attendance?report_type=per-member&dateStart=...&dateEnd=...&report-format=csv` |

### CSV parsing

`RevSportSyncService` parses each CSV with `Microsoft.VisualBasic.FileIO.TextFieldParser`. Each entity has an alias dictionary that maps every known header variant (normalised to lowercase, no spaces/underscores/hyphens) to the canonical `DataTable` column name. This tolerates RevSport changing or renaming CSV headers between exports.

### Known seasons

| SeasonId | Label |
|----------|-------|
| 43649 | 2025-26 (current) |
| 36538 | 2024-25 |
| 28884 | 2023-24 |
| 22247 | 2022-23 |
| 16773 | 2021-22 |

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

2 min / 5 min / 15 min / 1 hour / 4 hours / 12 hours / 24 hours (or any integer entered manually)

---

## Historical Backfill

A **Backfill** job type exists specifically for one-time historical data loads (e.g. importing 2 years of Deputy timesheets). Applies to date-filtered entities: Deputy Timesheets, Rosters; RevSport Events, EventAttendees.

### How it works

| Step | What happens |
|------|-------------|
| 1 | Operator creates a Backfill job with a date range (e.g. 01/01/2023 → today) and a chunk size (default 30 days) |
| 2 | Scheduler fires the job at the configured interval (default 2 min between chunks) |
| 3 | Each run pulls **one chunk** via the date-filtered query |
| 4 | `SyncCursor` in `dbo.Jobs` advances to the next chunk start |
| 5 | When `SyncCursor > SyncToDate`, the job auto-disables itself |

### Resilience

- `SyncCursor` is written to the database **after each successful chunk**, not at the end. If the app is closed mid-backfill, it resumes from the last completed chunk on next restart.
- Deputy syncs use MERGE (upsert); RevSport event syncs use DELETE+insert per range window.

### Jobs grid progress indicator

The **Schedule** column in the Jobs grid shows `Backfill XX%` — calculated from `(cursor − fromDate) / (toDate − fromDate)`.

---

## UI Panels

| Panel | Purpose |
|-------|---------|
| Dashboard | Row-count stats (4 boxes) + last 20 job runs |
| Jobs | Job CRUD grid + per-job history; "Run Now" button |
| Deputy | Tabbed read-only grids: Timesheets (date filter), Employees, Op Units, Work Types |
| RevSport | Tabbed read-only grids: Members (season picker), Events (date filter), Event Attendees (date filter) |
| Settings | SQL connection string editor + API config key/value store |
| Logs | Filterable run history (date range + status) |

---

## Adding a Future Data Source

1. Create a new SQL schema (`IF NOT EXISTS … EXEC('CREATE SCHEMA xxx')`) + tables in `DatabaseService.EnsureSchema()`
2. Create `Services\XxxApiService.vb` and `Services\XxxSyncService.vb`
3. Add a `Case "Xxx"` branch in `SchedulerService.DispatchJob` calling a new `DispatchXxx` function
4. Add config key defaults in `SettingsPanel.SeedDefaults()`
5. Add the new `SourceType` to `JobEditForm.cboSource.Items` and entity types to `cboEntity.Items`
6. Create `Forms\XxxPanel.vb` + `XxxPanel.Designer.vb`, wire into `MainForm.ShowPanel` and `MainForm.InitNavTree`
