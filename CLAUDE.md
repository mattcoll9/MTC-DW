# MTC-DW

A Windows desktop application (WinForms, VB.NET, .NET Framework 4.8) that pulls data from external APIs and loads it into a SQL Server data warehouse. The scheduler runs jobs on a configurable cadence; each job syncs a specific entity type (timesheets, employees, etc.) from a source API (currently Deputy) into the `dbo` / `deputy` schemas.

## Project Structure

```
MTC-DW/
├── source/MTC-DW/
│   ├── MTC-DW.vbproj              ← Project file (old-style, NOT SDK-style)
│   ├── Program.vb                 ← Entry point
│   ├── AppState.vb                ← Global shared state
│   ├── Models/
│   │   ├── AppConfig.vb
│   │   ├── JobDefinition.vb
│   │   ├── JobHistory.vb
│   │   └── Deputy/               ← DpTimesheet, DpEmployee, DpOperationalUnit, DpWorkType
│   ├── Services/
│   │   ├── DatabaseService.vb    ← SQL Server access + schema bootstrap (EnsureSchema)
│   │   ├── DeputyApiService.vb   ← Deputy REST API client
│   │   ├── DeputySyncService.vb  ← Orchestrates Deputy → SQL sync
│   │   └── SchedulerService.vb  ← Job scheduling / timer loop
│   └── Forms/
│       ├── MainForm              ← Shell with nav sidebar
│       ├── DashboardPanel        ← Job history grid
│       ├── JobsPanel             ← Job list + CRUD toolbar
│       ├── JobEditForm           ← Add / edit a job
│       ├── DeputyPanel           ← Deputy data preview tabs
│       ├── LogsPanel             ← Run log viewer
│       └── SettingsPanel         ← Connection string + config keys
└── claude.md
```

## Rules

- **Language**: VB.NET (.NET Framework 4.8)
- **Build tool**: Visual Studio only — never `dotnet build`. This is an old-style `.vbproj` targeting `v4.8`; the .NET SDK CLI does not handle `MyType=WindowsForms` global imports correctly and will produce false errors.
- **Minimal code**: simplest direct solution. No unnecessary variables, no defensive handling for impossible cases.
- **Edit over create**: prefer editing existing files. Only create new files when structure genuinely requires it.
- **No new docs unless asked**: don't generate `*.md` files unless explicitly requested.
- **No speculative abstractions**: don't design for hypothetical future requirements.
- **Version bump**: increment the build number in `My Project/AssemblyInfo.vb` (`AssemblyVersion` and `AssemblyFileVersion`) on every code change.
- **Verify Deputy endpoints in Postman first**: never write sync code for a new Deputy entity type until the user has confirmed the endpoint and response shape in Postman. Push back if asked to add an unverified entity.

## VB.NET Gotchas (things that look like valid code but aren't)

| C# / wrong | VB.NET correct |
|---|---|
| `x = Select Case y ... End Select` | `Select Case` is a **statement**, not an expression — put the assignment inside each `Case` |
| `"multi` *(newline)* `line string"` | VB.NET has no multi-line string literals — keep SQL/long strings on one line, or concatenate with `& _` |
| `value ?? fallback` | `If(value, fallback)` |
| `value?.Prop ?? fallback` | `If(value?.Prop, fallback)` |
| `?.` null-conditional | Supported in VB 14+ — this one is fine |

### Newtonsoft.Json date token gotcha

When Newtonsoft parses an ISO date string (e.g. `"2026-04-01"`), it auto-converts it to a `JTokenType.Date` token. Calling `.Value(Of String)()` on a Date token does **not** return the original string — it returns a locale-formatted representation that `DateTime.TryParse` may fail on. Always use `ParseJDateTime` / `ParseJDate` helpers (in `DeputySyncService`) instead of `obj.Value(Of String)("DateField")` for any field that could be a date.

## Key Patterns

### DB access (DatabaseService)

```vb
Using conn = _db.GetConnection()
    Using cmd As New SqlCommand(sql, conn)
        cmd.Parameters.AddWithValue("@Param", value)
        cmd.ExecuteNonQuery()
    End Using
End Using
```

### Row colouring in DataGridView

```vb
Private Sub ColourRows(dgv As DataGridView)
    For Each row As DataGridViewRow In dgv.Rows
        Dim status = If(row.Cells("Status")?.Value?.ToString(), "")
        Select Case status
            Case "Success" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(220, 255, 220)
            Case "Failed"  : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 220, 220)
            Case "Running" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 255, 200)
            Case Else      : row.DefaultCellStyle.BackColor = Drawing.Color.White
        End Select
    Next
End Sub
```

### Null-safe JSON helpers (DeputySyncService)

All Deputy JSON fields can be `null`. Never call `.Value(Of T)()` directly — use these shared helpers:

```vb
SafeBool(obj, "FieldName")       ' → Boolean (false if null)
SafeLong(obj, "FieldName")       ' → Long (0 if null)
SafeDecimal(obj, "FieldName")    ' → Decimal (0 if null)
ParseJDateTime(obj, "FieldName") ' → DateTime? (handles JTokenType.Date + ISO strings)
ParseJDate(obj, "FieldName")     ' → Date? (date portion only)
ParseMealbreakMins(obj, "Field") ' → Decimal? minutes (Mealbreak stores duration as time-of-day on a datetime)
```

For nullable integer foreign-key fields (e.g. `Company`, `OperationalUnit`, `Roster` on an entity):
```vb
Dim companyId As Long? = Nothing
Dim cToken = obj("Company")
If cToken IsNot Nothing AndAlso cToken.Type = JTokenType.Integer Then companyId = cToken.Value(Of Long)()
```

### Deputy API request patterns

**Date-filtered paged query** (Timesheets, Rosters):
```
POST resource/Timesheet/QUERY
{"search":{"mdFrom":{"field":"Date","type":"ge","data":"2024-01-01"},"mdTo":{"field":"Date","type":"le","data":"2024-01-31"}}}
```

**Get all** (Employees, OperationalUnits, Company, etc.):
```
POST resource/Employee/QUERY   (with {"search":{}})
-- or --
GET  resource/Employee         (via GetAll helper, which POSTs {"search":{}})
```

### Known Deputy field quirks

| Entity | Field | Notes |
|---|---|---|
| Timesheet | `StartTime`, `EndTime` | Unix epoch integers — always use `StartTimeLocalized` / `EndTimeLocalized` instead |
| Timesheet | `Mealbreak` | Datetime where the **time portion** = break duration (e.g. `T00:30:00` = 30 min) |
| Timesheet | `Date` | Newtonsoft parses as `JTokenType.Date` — use `ParseJDate` |
| Employee | `Position` | Returns an integer ID, not an object — guard with `posObj.Type = JTokenType.Object` before accessing sub-fields |
| Employee | `Company` | Integer ID (not object) |
| OperationalUnit | `Company` | Integer ID (not object); **no `Code` field** — `PayrollExportName` is the code equivalent |
| OperationalUnit | `CompanyCode`, `CompanyName` | Denormalised onto the OU response — read directly from `obj` |

### Backfill cursor behaviour

`SyncCursor` tracks progress through the date range chunk by chunk. When a backfill job completes, `UpdateBackfillCursor` sets `IsEnabled = False` and clears `NextRunTime`. To restart or change the date range:
- Edit the job and save — the save code always resets `SyncCursor = Nothing`, which forces the next run to start from `SyncFromDate`.
- Make sure **Enabled** is checked before saving.

### Schema bootstrap (DatabaseService.EnsureSchema)

SQL strings in the `statements` array must be **single-line** — VB.NET does not support multi-line string literals. Collapse all CREATE TABLE / ALTER TABLE statements to one line each.

## Build / Run

```
# Open and build
Open source/MTC-DW/MTC-DW.vbproj (or MTC-DW.sln) in Visual Studio
Build → Build Solution  (Ctrl+Shift+B)

# Run
Debug → Start Debugging  (F5)
```

> **Before asking Claude to edit files:** do `Ctrl+Shift+S` (Save All) in Visual Studio first. VS holds in-memory copies of open files; unsaved changes suppress auto-reload and VS will compile its stale version instead of Claude's edits.

## External Services / Dependencies

- **SQL Server** — connection string stored in `app.config` (`AppConfig` model reads it). Schema lives in `dbo` (jobs, config, history) and `deputy` (entity tables). `DatabaseService.EnsureSchema()` bootstraps all tables on startup — idempotent, safe to re-run.
- **Deputy API** — REST API, auth token stored in app config. `DeputyApiService` handles HTTP; `DeputySyncService` orchestrates paging + upsert into SQL.
- **Newtonsoft.Json 13.0.3** — only external NuGet dependency; used for Deputy API response deserialisation.

## What NOT to do

- Don't use `dotnet build` — always use Visual Studio or `msbuild.exe` from a VS Developer Command Prompt.
- Don't add `<Reference>` entries without also checking whether a matching `<Import>` is needed in the `.vbproj` — references link the assembly, imports expose its namespaces globally across all `.vb` files.
- Don't add new NuGet packages without asking.
- Don't commit directly to `main` for anything beyond minor fixes — prefer a feature branch.
- Don't modify `*.Designer.vb` files by hand — they are regenerated by the VS WinForms designer.
