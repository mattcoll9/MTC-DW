# MTC Data Warehouse — Configuration Guide

## Prerequisites

| Requirement | Version |
|-------------|---------|
| Windows | 10 / 11 |
| Visual Studio | 2019 or 2022 (any edition including Community) |
| .NET Framework | 4.8 (included with Windows 10 1903+) |
| SQL Server | 2019 (any edition including Express/Developer) |
| Deputy account | API access enabled |
| RevSport account | Portal login credentials (+ TOTP seed if 2FA is enabled) |

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
The database (`MTC_DataWarehouse`) must exist before the app starts — create it in SSMS:

```sql
CREATE DATABASE MTC_DataWarehouse;
```

Alternatively, update the connection string from within the app after first launch (Settings panel → Save will update app.config).

### 3. Build and run

Press **F5** in Visual Studio. On first run the app will:
1. Connect to SQL Server
2. Create the `deputy` and `revsport` schemas and all tables automatically
3. Open the Dashboard

If the connection fails, the Settings panel opens automatically.

---

## Entering Deputy API Credentials

1. Click **Settings** in the nav tree
2. In the **API Configuration** section, add each key and value, then click **Save**:

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

## Entering RevSport Credentials

RevSport uses web portal login rather than an API token. The app drives the login flow automatically using the credentials stored in `dbo.Config`.

1. Click **Settings** in the nav tree
2. Add each key and value, then click **Save**:

| Key | Value |
|-----|-------|
| `RevSport.Email` | Your RevSport portal login email |
| `RevSport.Password` | Your RevSport portal login password |
| `RevSport.TotpSeed` | Base-32 TOTP seed — **only required if 2FA is enabled on the account**; leave empty otherwise |
| `RevSport.SeasonId` | Season ID to use for member sync (default `43649` = 2025-26; see Known Seasons below) |
| `RevSport.EventsDaysBack` | How many days back to pull events/attendees when no date range is set on the job (default `90`) |

**Finding the TOTP seed:**  
When setting up 2FA in RevSport, the portal presents a QR code. Most authenticator apps allow you to view the underlying secret key — it will be a string of uppercase letters and digits. This is the value to enter for `RevSport.TotpSeed`.

**Known Season IDs:**

| SeasonId | Season |
|----------|--------|
| 43649 | 2025-26 (current) |
| 36538 | 2024-25 |
| 28884 | 2023-24 |
| 22247 | 2022-23 |
| 16773 | 2021-22 |

---

## Creating Deputy Jobs

1. Click **Jobs** in the nav tree
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

**Recommended job set for Deputy:**

| Job Name | Entity | Interval |
|----------|--------|----------|
| Deputy — Employees | Employees | 1440 min (24h) |
| Deputy — Operational Units | OperationalUnits | 1440 min (24h) |
| Deputy — Company | Company | 1440 min (24h) |
| Deputy — Timesheets | Timesheets | 240 min (4h) |
| Deputy — Rosters | Rosters | 240 min (4h) |

Run the Employees and OperationalUnits jobs first so lookup tables are populated before timesheets sync.

---

## Creating RevSport Jobs

**Recommended job set for RevSport:**

| Job Name | Source | Entity | Schedule | Interval |
|----------|--------|--------|----------|----------|
| RevSport — Members | RevSport | Members | Recurring | 1440 min (24h) |
| RevSport — Events | RevSport | Events | Recurring | 1440 min (24h) |
| RevSport — Event Attendees | RevSport | EventAttendees | Recurring | 1440 min (24h) |

- **Members** uses `RevSport.SeasonId` from config — no date range required on the job.
- **Events** and **EventAttendees** use the job's `SyncFromDate`/`SyncToDate` if set, otherwise fall back to today minus `RevSport.EventsDaysBack` days.

To load events for a specific period, set Schedule to **Backfill** and provide a date range.

---

## Running a Historical Backfill (Deputy)

Use this when you need to load more history than the rolling `SyncDaysBack` window covers.

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
5. When complete, the job auto-disables itself (`Enabled = No`)

**Tips:**
- Smaller chunks (14 days) are safer if the Deputy API is slow or rate-limited
- The backfill is restartable: close the app at any time and it resumes from the last completed chunk
- Run `Employees` and `OperationalUnits` jobs **first** so FK lookups in timesheets resolve correctly

---

## Viewing Synced Data

### Deputy data

- Click **Deputy** in the nav tree
- **Timesheets** tab: set From/To date range and click **Load**
- **Employees**, **Operational Units**, and **Work Types** tabs load automatically on selection

### RevSport data

- Click **RevSport** in the nav tree
- **Members** tab: select a season from the dropdown and click **Load**
- **Events** and **Event Attendees** tabs: set From/To date range and click **Load**

---

## Checking Logs

- Click **Logs** in the nav tree
- Filter by date range and/or status (All / Success / Failed / Running)
- Row colours: green = Success, red = Failed, yellow = Running

---

## Updating the Connection String at Runtime

Settings panel → **SQL Server Connection** section:
1. Edit the connection string in the text box
2. Click **Test** to verify
3. Click **Save** — this writes to `app.config` and takes effect immediately

A restart is recommended after changing to a new database, as schema initialisation only runs on startup.

---

## Troubleshooting

| Symptom | Check |
|---------|-------|
| "Cannot connect" on startup | SQL Server running? Instance name correct? Firewall? |
| Jobs not firing on schedule | App must remain open. Check NextRunTime in Jobs grid. |
| Deputy sync returns 0 records | Check `Deputy.BaseUrl` ends with `/api/v1/`. Check `Deputy.OAuthToken` is correct. |
| RevSport login fails | Check `RevSport.Email` and `RevSport.Password`. If 2FA is enabled, ensure `RevSport.TotpSeed` is set to the correct base-32 secret. |
| RevSport returns HTML instead of CSV | Session expired and re-auth also failed — check credentials. |
| RevSport sync returns 0 records | The date range may return no events, or the season ID may be incorrect. Check the portal manually. |
| "Source type not supported" error | SourceType in job row doesn't match any `Case` in `SchedulerService.DispatchJob`. |
| Schema creation error | Ensure the database exists and your Windows account has `db_owner` or `ddl_admin`. |
