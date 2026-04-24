# MTC Data Warehouse — Configuration Guide

## Prerequisites

| Requirement | Version |
|-------------|---------|
| Windows | 10 / 11 |
| Visual Studio | 2019 or 2022 (any edition including Community) |
| .NET Framework | 4.8 (included with Windows 10 1903+) |
| SQL Server | 2019 (any edition including Express/Developer) |
| Deputy account | API access enabled |

---

## First-Time Setup

### 1. Open the solution

Open `source\MTC-DW.sln` in Visual Studio. NuGet will automatically restore the `Newtonsoft.Json` package on the first build.

### 2. Set the SQL Server connection string

Edit `source\MTC-DW\app.config`. Find:

```xml
<add name="MTCDW"
     connectionString="Server=localhost;Database=MTC_DataWarehouse;Integrated Security=True;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

Update `Server=` to your SQL Server instance name (e.g. `.\SQLEXPRESS`, `MYSERVER\SQL2019`).  
The database (`MTC_DataWarehouse`) will be **created automatically** if it exists; the app only creates tables. Create the database first in SSMS:

```sql
CREATE DATABASE MTC_DataWarehouse;
```

Alternatively, update the connection string from within the app after first launch (Settings panel → Save will update app.config).

### 3. Build and run

Press **F5** in Visual Studio. On first run the app will:
1. Connect to SQL Server
2. Create the `deputy` schema and all tables automatically
3. Open the Dashboard

If the connection fails, the Settings panel opens automatically.

---

## Entering Deputy API Credentials

1. Click **Settings** in the toolbar (or nav tree)
2. In the **API Configuration** section, type the key and value, then click **Save**:

| Key | Value |
|-----|-------|
| `Deputy.BaseUrl` | `https://YOUR_INSTANCE.au.deputy.com/api/v1/` |
| `Deputy.OAuthToken` | Your Deputy OAuth token (without the "OAuth " prefix) |
| `Deputy.SyncDaysBack` | Number of days of timesheets to pull (default: `90`) |

**Finding your Deputy OAuth token:**  
In Deputy → Settings → API → Permanent Tokens. Copy the token value only (not the header).

**Finding your Deputy base URL:**  
It is the URL you use to log into Deputy, e.g. `https://mycompany.au.deputy.com`, with `/api/v1/` appended.

---

## Creating Your First Job

1. Click **Jobs** in the toolbar
2. Click **Add Job** in the toolbar strip
3. Fill in the dialog:

| Field | Recommended value |
|-------|-----------------|
| Job Name | `Deputy — Timesheets` |
| Source | `Deputy` |
| Entity | `Timesheets` |
| Schedule | `Recurring` |
| Interval | `240` (every 4 hours) |
| Next Run | Set to a time a few minutes from now |
| Enabled | Checked |

4. Click **OK**
5. To test immediately: select the job row and click **Run Now**
6. Watch the status bar at the bottom of the window and check the **Logs** panel

**Recommended job set for Deputy:**

| Job Name | Entity | Interval |
|----------|--------|----------|
| Deputy — Timesheets | Timesheets | 240 min (4h) |
| Deputy — Employees | Employees | 1440 min (24h) |
| Deputy — Operational Units | OperationalUnits | 1440 min (24h) |
| Deputy — Work Types | WorkTypes | 1440 min (24h) |

Run the non-Timesheet jobs first so that the lookup tables are populated before timesheets sync.

---

## Running a Historical Backfill

Use this when you need to load more history than the rolling `SyncDaysBack` window covers — e.g. the first time you set up Deputy, or to backfill 2 years of timesheets.

1. Click **Jobs** → **Add Job**
2. Set the fields as follows:

| Field | Value |
|-------|-------|
| Job Name | `Deputy — Timesheets Backfill 2023–2024` |
| Source | `Deputy` |
| Entity | `Timesheets` |
| Schedule | **Backfill (historical)** |
| Pull From | `01/01/2023` (or your desired start) |
| To | today's date |
| Chunk (days) | `30` *(pull one month per run)* |
| Interval (min) | `2` *(2 minutes between chunks)* |
| First Run | a few minutes from now |
| Enabled | Checked |

3. Click **OK**, then select the row and click **Run Now** to start immediately
4. Watch the **Schedule** column — it will show `Backfill 4%`, `Backfill 8%` etc. as chunks complete
5. Check the **Logs** panel to see each chunk's record count
6. When complete, the job auto-disables itself (`Enabled = No`)

**Tips:**
- Smaller chunks (14 days) are safer if the Deputy API is slow or rate-limited
- The backfill is restartable: close the app at any time and it will resume from the last completed chunk
- Run the `Employees`, `OperationalUnits`, and `WorkTypes` jobs **first** so FK lookups in timesheets resolve correctly

---

## Viewing Synced Data

- Click **Deputy** in the toolbar
- The **Timesheets** tab shows a date-filtered grid (set From/To and click Load)
- The **Employees**, **Operational Units**, and **Work Types** tabs load automatically when the tab is selected

---

## Checking Logs

- Click **Logs** in the toolbar
- Filter by date range and/or status (All / Success / Failed / Running)
- Row colours: green = Success, red = Failed, yellow = Running

---

## Updating the Connection String at Runtime

Settings panel → **SQL Server Connection** section:
1. Edit the connection string in the text box
2. Click **Test** to verify
3. Click **Save** — this writes to `app.config` and takes effect immediately

A restart is recommended after changing to a new database, as the schema initialisation only runs on startup.

---

## Adding a New Data Source (Developer Guide)

1. **Create the schema and tables** — add DDL to `DatabaseService.EnsureSchema()` following the same `IF NOT EXISTS` pattern
2. **Create `Services\XxxApiService.vb`** — model after `DeputyApiService.vb`; implement `GetAllPaged` or equivalent
3. **Create `Services\XxxSyncService.vb`** — model after `DeputySyncService.vb`; one `Async Function SyncXxx() As Task(Of Integer)` per entity
4. **Register in the scheduler** — add a `Case "Xxx"` branch in `SchedulerService.DispatchJob`
5. **Add config keys** — add defaults to `SettingsPanel.SeedDefaults()` (e.g. `Xxx.BaseUrl`, `Xxx.ApiKey`)
6. **Add a UI panel** — create `Forms\XxxPanel.vb` + `XxxPanel.Designer.vb`, wire into `MainForm.ShowPanel` and `MainForm.InitNavTree`
7. **Add to Jobs dialog** — add the new `SourceType` string to `JobEditForm.cboSource.Items`

No changes to any existing files (other than steps 4, 5, 6, 7) are required.

---

## Future: Separate Database per Source

The design supports this without code changes. To move the `deputy` schema to a separate database:

1. In Settings, add a config key: `Deputy.ConnectionString` with the new database's connection string
2. In `SchedulerService.DispatchDeputy`, read this key and pass it to `DatabaseService` instead of the default connection
3. In `DatabaseService.EnsureSchema`, move the `deputy.*` DDL to a separate method called with the deputy-specific connection

---

## Troubleshooting

| Symptom | Check |
|---------|-------|
| "Cannot connect" on startup | SQL Server running? Instance name correct? Firewall? |
| Jobs not firing on schedule | App must remain open. Check NextRunTime in Jobs grid. |
| Deputy sync returns 0 records | Check `Deputy.BaseUrl` ends with `/api/v1/`. Check `Deputy.OAuthToken` is correct. |
| "Source type not supported" error | SourceType in job row doesn't match any `Case` in `SchedulerService.DispatchJob`. |
| Schema creation error | Ensure the database exists and your Windows account has `db_owner` or `ddl_admin`. |
