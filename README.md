# WeeklyReportWS — REST API Service

> **ASP.NET Web API 2** self-hosted REST service for the Weekly Action Tracking System.  
> Runs on **.NET Framework 4.8** via OWIN, backed by **SQL Server** and **Dapper**.

---

## 📸 Screenshots

### Service Running in Console
![Service Console](docs/screenshots/service-console.png)

### Health Check Response
![Health Check](docs/screenshots/health-check.png)

---

## 🏗️ Architecture

```
┌──────────────────────────────────────────────────────────┐
│         WeeklyReportWS.exe  (OWIN Self-Host)             │
│                                                          │
│  Program.cs                                              │
│    └─ WebApp.Start<Startup>(http://0.0.0.0:4443)        │
│                                                          │
│  Startup.cs (OWIN Pipeline)                              │
│    ├─ CORS  ←  App.config > AllowedOrigins               │
│    ├─ GlobalExceptionFilter  (stack trace suppression)   │
│    ├─ JSON  ←  Newtonsoft.Json, PascalCase               │
│    └─ HttpConfiguration + Attribute Routing              │
│                                                          │
│  Controllers/  (10 ApiControllers)                       │
│    └─ DbConnectionFactory.CreateConnection()             │
│         ├─ env: WEEKLY_REPORT_CONNECTION_STRING          │
│         └─ fallback: App.config DefaultConnection        │
└──────────────────────────────────┬───────────────────────┘
                                   │ Microsoft.Data.SqlClient
                                   ▼
                            SQL Server (WeeklyReport)
```

---

## 🛠️ Tech Stack

| Package | Version | Purpose |
|---------|---------|---------|
| .NET Framework | 4.8 | Runtime |
| ASP.NET Web API 2 (OWIN Self-Host) | 5.2.9 | HTTP server + REST routing |
| Microsoft.Owin.Cors | 4.2.2 | CORS policy |
| Dapper | 2.1.35 | Micro ORM / parameterized queries |
| Microsoft.Data.SqlClient | 5.2.1 | SQL Server driver |
| Newtonsoft.Json | 13.0.3 | JSON serialization (PascalCase) |

---

## 📁 Project Structure

```
weekly-report-net-ws/
├── Controllers/
│   ├── ActionsController.cs            ← CRUD + PATCH status + soft-delete
│   ├── ActionItemsController.cs        ← Action sub-items (text / image)
│   ├── ActionStatusesController.cs     ← Status definitions
│   ├── ActionStatusHistoryController.cs← Status change audit trail
│   ├── ActionTypesController.cs        ← Action type lookup
│   ├── DepartmentsController.cs        ← Department lookup
│   ├── LinesController.cs              ← Line (EVP) lookup
│   ├── UnitsController.cs              ← Unit lookup
│   ├── UsersController.cs              ← User lookup + Windows identity
│   ├── WeeksController.cs              ← Week lookup
│   └── HealthController.cs             ← GET /health
├── Data/
│   └── DbConnectionFactory.cs          ← Static connection factory
├── Filters/
│   └── GlobalExceptionFilter.cs        ← Hides stack traces from clients
├── Models/
│   └── Models.cs                       ← All entity + request models
├── Program.cs                          ← Entry point, reads Port from config
├── Startup.cs                          ← OWIN pipeline setup
├── App.config                          ← Connection string + settings
└── WeeklyReportWS.csproj
```

---

## ⚙️ Configuration (`App.config`)

```xml
<configuration>
  <connectionStrings>
    <add name="DefaultConnection"
         connectionString="Server=YOUR_SERVER\SQLEXPRESS;Database=WeeklyReport;
                           User Id=your_user;Password=your_password;
                           Encrypt=True;TrustServerCertificate=True;" />
  </connectionStrings>
  <appSettings>
    <!-- Listening port -->
    <add key="Port" value="4443" />
    <!-- Comma-separated allowed CORS origins. Empty = allow all (dev only) -->
    <add key="AllowedOrigins" value="http://localhost:3000" />
  </appSettings>
</configuration>
```

### Connection String Priority

```
1. Environment variable  →  WEEKLY_REPORT_CONNECTION_STRING
2. App.config            →  DefaultConnection
3. Exception thrown      →  Application will not start
```

**Setting via environment variable (recommended for production):**
```powershell
$env:WEEKLY_REPORT_CONNECTION_STRING = "Server=YOUR_SERVER\SQLEXPRESS;Database=WeeklyReport;User Id=user;Password=pass;Encrypt=True;TrustServerCertificate=True;"
```

---

## 🚀 Build & Run

### Prerequisites

- .NET Framework 4.8 (pre-installed on Windows 10/11 and Windows Server 2019+)
- .NET SDK 6+ (for `dotnet` CLI) **or** Visual Studio 2019/2022
- SQL Server with the `WeeklyReport` database created

### Build

```bash
dotnet build WeeklyReportWS.csproj
```

Expected output:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Run

```bash
dotnet run --project WeeklyReportWS.csproj
```

```
Starting weekly-report-ws on http://0.0.0.0:4443/
Server started. Press Enter to stop...
```

### Health Check

```
GET http://localhost:4443/health
```

```json
{
  "status": "ok",
  "timestamp": "2026-04-09T09:30:00Z"
}
```

---

## 🔌 REST API Reference

Base URL: `http://localhost:4443`  
All responses are JSON with **PascalCase** property names.

---

### Actions — `GET /api/actions`

Returns a filtered list of actions (non-deleted).

**Query parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `weekNumber` | int | Filter by week number (1–52) |
| `year` | int | Filter by year |
| `userId` | int | Filter by user |
| `lineId` | int | Filter by line |
| `unitId` | int | Filter by unit |
| `statusId` | int | Filter by status |

**Response (200):**
```json
[
  {
    "ActionID": 101,
    "UserID": 1,
    "WeekID": 15,
    "WeekNumber": 15,
    "Year": 2026,
    "TypeID": 2,
    "TypeName": "Development",
    "ActionDate": "2026-04-14T00:00:00",
    "StatusID": 3,
    "StatusKey": "progress",
    "StatusLabel": "Progress",
    "ColorHex": "#10b981",
    "BgColorHex": "#ecfdf5",
    "FullName": "John Doe",
    "IsDeleted": false,
    "ActionItems": null
  }
]
```

---

### Actions — `GET /api/actions/{id}`

Returns a single action with its sub-items.

**Response (200):**
```json
{
  "ActionID": 101,
  "ActionItems": [
    { "ItemID": 1, "ActionID": 101, "SortOrder": 0, "ItemType": "text", "ItemValue": "Main action text" },
    { "ItemID": 2, "ActionID": 101, "SortOrder": 1, "ItemType": "text", "ItemValue": "Sub-item" }
  ]
}
```

---

### Actions — `POST /api/actions`

Creates a new action with sub-items in a single transaction.

**Request body:**
```json
{
  "UserID": 1,
  "WeekID": 15,
  "TypeID": 2,
  "ActionDate": "2026-04-14",
  "StatusID": null,
  "actionItems": [
    { "type": "text",  "value": "Main action text" },
    { "type": "text",  "value": "Sub-item" },
    { "type": "image", "value": "data:image/png;base64,iVBORw0..." }
  ]
}
```

**Response (201 Created):** Returns the created `Action` object.

---

### Actions — `PUT /api/actions/{id}`

Replaces action fields and sub-items in a single transaction. Old items are deleted and re-inserted.

**Request body:**
```json
{
  "WeekID": 15,
  "TypeID": 3,
  "ActionDate": "2026-04-15",
  "actionItems": [
    { "type": "text", "value": "Updated action text" }
  ]
}
```

**Response (200):** Returns the updated `Action` object.

---

### Actions — `PATCH /api/actions/{id}/status`

Updates only the status and writes an audit entry to `ActionStatusHistory`.

**Request body:**
```json
{
  "StatusID": 3,
  "ChangedBy": 1
}
```

> Send `"StatusID": null` to clear the status (Remove from Report).

**Response (200):**
```json
{ "ActionID": 101, "StatusID": 3 }
```

---

### Actions — `DELETE /api/actions/{id}`

Soft-deletes the action (`IsDeleted = 1`). The record is never physically removed.

**Response (204 No Content)**

---

### Action Sub-Items

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/actions/{actionId}/items` | List sub-items ordered by `SortOrder` |
| `POST` | `/api/actions/{actionId}/items` | Add a sub-item |
| `PUT` | `/api/actions/{actionId}/items/{itemId}` | Update a sub-item |
| `DELETE` | `/api/actions/{actionId}/items/{itemId}` | Delete a sub-item |

**POST body:**
```json
{ "ItemType": "text", "ItemValue": "New sub-item", "SortOrder": 1 }
```

---

### Status History

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/actions/{actionId}/status-history` | Full status change history |
| `POST` | `/api/actions/{actionId}/status-history` | Insert manual history entry |
| `DELETE` | `/api/actions/{actionId}/status-history/{historyId}` | Remove history entry |

**GET Response:**
```json
[
  {
    "HistoryID": 5,
    "ActionID": 101,
    "StatusID": 3,
    "StatusKey": "progress",
    "StatusLabel": "Progress",
    "ColorHex": "#10b981",
    "ChangedBy": 1,
    "ChangedByName": "John Doe",
    "ChangedAt": "2026-04-09T10:15:00"
  }
]
```

---

### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/users` | All users |
| `GET` | `/api/users/{id}` | User by ID |
| `GET` | `/api/users/windowname/{windowName}` | User by Windows identity |
| `GET` | `/user/getuserdata?windowName=` | Legacy lookup (returns 404 if not found) |
| `POST` | `/api/users` | Create user |
| `PUT` | `/api/users/{id}` | Update user |

**User response model:**
```json
{
  "UserID": 1,
  "WindowName": "DOMAIN\\jdoe",
  "FullName": "John Doe",
  "Title": "Software Engineer",
  "PositionNumber": 2,
  "DepartmentID": 3,
  "DepartmentName": "API Team",
  "UnitID": 2,
  "UnitName": "Backend Development",
  "LineID": 1,
  "LineName": "Application Development"
}
```

> `PositionNumber`: `1` = Team Member | `2` = Manager | `3` = Unit Manager | `4` = EVP | `5` = General Manager

---

### Reference / Lookup Endpoints

| Method | Endpoint | Query Params | Description |
|--------|----------|--------------|-------------|
| `GET` | `/api/weeks` | `year` | Weeks for a given year |
| `GET` | `/api/lines` | — | All lines |
| `POST` | `/api/lines` | — | Create line |
| `PUT` | `/api/lines/{id}` | — | Update line |
| `DELETE` | `/api/lines/{id}` | — | Delete line |
| `GET` | `/api/units` | `lineId` | Units (optionally filtered) |
| `POST` | `/api/units` | — | Create unit |
| `GET` | `/api/departments` | `unitId`, `lineId` | Departments (optionally filtered) |
| `POST` | `/api/departments` | — | Create department |
| `GET` | `/api/action-types` | — | Action types |
| `POST` | `/api/action-types` | — | Create action type |
| `PUT` | `/api/action-types/{id}` | — | Update action type |
| `DELETE` | `/api/action-types/{id}` | — | Delete action type |
| `GET` | `/api/action-statuses` | — | Action statuses |
| `POST` | `/api/action-statuses` | — | Create action status |
| `PUT` | `/api/action-statuses/{id}` | — | Update action status |
| `GET` | `/health` | — | Health check |

---

## 🔒 Security

| Concern | Mitigation |
|---------|------------|
| Hardcoded credentials | Connection string read from env var first; `App.config` as fallback only |
| SQL Injection | All queries use Dapper parameterized parameters — no raw string concatenation |
| Stack trace leakage | `GlobalExceptionFilter` returns `{ "error": "An internal server error occurred." }` |
| Unrestricted CORS | `AllowedOrigins` in `App.config` restricts to known origins |
| Plaintext DB traffic | `Encrypt=True` enforces TLS on the SQL connection |
| Permanent data loss | Actions use soft-delete (`IsDeleted=1`); physical records are never removed |

---

## 📦 Models Reference

### Request / Response Models

| Model | Used In | Key Fields |
|-------|---------|------------|
| `Action` | Actions GET/POST/PUT | `ActionID`, `UserID`, `WeekID`, `TypeID`, `ActionDate`, `StatusID`, `ActionItems[]` |
| `CreateActionRequest` | POST /api/actions | `UserID`, `WeekID`, `TypeID`, `ActionDate`, `actionItems[]` |
| `UpdateActionRequest` | PUT /api/actions/{id} | `WeekID`, `TypeID`, `ActionDate`, `actionItems[]` |
| `PatchActionStatusRequest` | PATCH …/status | `StatusID` (nullable), `ChangedBy` |
| `ActionItem` | Sub-items | `ItemID`, `ActionID`, `SortOrder`, `ItemType` (`text`/`image`), `ItemValue` |
| `ActionStatus` | Statuses | `StatusID`, `StatusKey`, `StatusLabel`, `ColorHex`, `BgColorHex` |
| `ActionStatusHistory` | History | `HistoryID`, `StatusID`, `ChangedBy`, `ChangedAt`, `ChangedByName` |
| `User` | Users | `UserID`, `WindowName`, `FullName`, `PositionNumber`, `LineID`, `UnitID`, `DepartmentID` |
| `Week` | Weeks | `WeekID`, `WeekNumber`, `Year` |
| `Line` / `Unit` / `Department` | Lookups | Respective ID + Name fields |

---

## 🧑‍💻 Developer Notes

### Adding a new controller

1. Create `Controllers/MyController.cs` extending `ApiController`
2. Decorate with `[RoutePrefix("api/my-resource")]`
3. Use `DbConnectionFactory.CreateConnection()` for DB access
4. Return `IHttpActionResult` (`Ok()`, `NotFound()`, `Content(HttpStatusCode.Created, obj)`)

```csharp
[RoutePrefix("api/example")]
public class ExampleController : ApiController
{
    [HttpGet, Route("")]
    public async Task<IHttpActionResult> GetAll()
    {
        using var con = DbConnectionFactory.CreateConnection();
        var rows = await con.QueryAsync<MyModel>("SELECT * FROM tbl_example");
        return Ok(rows);
    }
}
```

### Connection factory

```csharp
// Priority: env var → App.config → InvalidOperationException
using var con = DbConnectionFactory.CreateConnection();
await con.OpenAsync();
```

### Error handling

Unhandled exceptions are caught by `GlobalExceptionFilter` — no stack traces are exposed to clients. Log errors server-side as needed.

---

## 📄 License

This project was developed for internal corporate use.
