using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IDbConnectionFactory _db;
        public UsersController(IDbConnectionFactory db) { _db = db; }

        private const string UserSelect = @"
            SELECT u.UserID, u.WindowsName, u.FullName, u.Title, u.PositionNumber,
                   u.DepartmentID, d.DepartmentName,
                   u.UnitID,       un.UnitName,
                   u.LineID,       l.LineName,
                   u.CreatedAt, u.UpdatedAt
            FROM tbl_weekly_report_Users u
            LEFT JOIN tbl_weekly_report_Departments d  ON d.DepartmentID = u.DepartmentID
            LEFT JOIN tbl_weekly_report_Units       un ON un.UnitID      = u.UnitID
            LEFT JOIN tbl_weekly_report_Lines       l  ON l.LineID       = u.LineID";

        // GET /api/users
        [HttpGet, Route("api/users")]
        public async Task<IHttpActionResult> GetAll()
        {
            using var con = _db.CreateConnection();
            var rows = await con.QueryAsync<User>(UserSelect + " ORDER BY u.FullName");
            return Ok(rows);
        }

        // GET /user/getuserdata?windowName=  (legacy)
        [HttpGet, Route("user/getuserdata")]
        public async Task<IHttpActionResult> GetUserData(string? windowsName = null)
        {
            if (string.IsNullOrWhiteSpace(windowsName))
                return BadRequest("windowsName is required");
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.WindowsName = @windowsName", new { windowsName });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // GET /api/users/windowname/:windowName
        [HttpGet, Route("api/users/windowsname/{windowsName}")]
        public async Task<IHttpActionResult> GetByWindowsName(string windowsName)
        {
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.WindowsName = @windowsName", new { windowsName });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // GET /api/users/me  – resolved from Windows Authentication (User.Identity.Name)
        [Authorize]
        [HttpGet, Route("api/users/me")]
        public async Task<IHttpActionResult> GetMe()
        {
            var rawName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(rawName))
                return Unauthorized();

            // IIS returns DOMAIN\username – try both forms against the DB
            var shortName = rawName.Contains("\\") ? rawName.Split('\\')[1] : rawName;

            // Uppercase with en-US/Invariant rules to avoid Turkish i→İ conflicts
            var rawUpper   = rawName.ToUpper(CultureInfo.InvariantCulture);
            var shortUpper = shortName.ToUpper(CultureInfo.InvariantCulture);

            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE UPPER(u.WindowsName) = @rawUpper OR UPPER(u.WindowsName) = @shortUpper",
                new { rawUpper, shortUpper });
            if (row == null)
                return Content(HttpStatusCode.NotFound,
                    new { WindowsName = rawName, ShortName = shortName });
            return Ok(row);
        }

        // GET /api/users/:id
        [HttpGet, Route("api/users/{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.UserID = @id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/users
        [HttpPost, Route("api/users")]
        public async Task<IHttpActionResult> Create([FromBody] CreateUserRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.WindowsName) || string.IsNullOrWhiteSpace(body.FullName))
                return BadRequest("WindowsName and FullName are required");
            using var con = _db.CreateConnection();
            await con.ExecuteAsync(@"
                INSERT INTO tbl_weekly_report_Users (WindowsName,FullName,DepartmentID,UnitID,LineID,Title,PositionNumber)
                VALUES (@WindowsName,@FullName,@DepartmentID,@UnitID,@LineID,@Title,@PositionNumber)",
                new { body.WindowsName, body.FullName, body.DepartmentID, body.UnitID, body.LineID, body.Title, body.PositionNumber });
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.WindowsName = @windowsName", new { windowsName = body.WindowsName });
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/users/:id
        [HttpPut, Route("api/users/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] UpdateUserRequest body)
        {
            using var con = _db.CreateConnection();
            await con.ExecuteAsync(@"
                UPDATE tbl_weekly_report_Users
                SET FullName=@FullName, DepartmentID=@DepartmentID, UnitID=@UnitID,
                    LineID=@LineID, Title=@Title, PositionNumber=@PositionNumber, UpdatedAt=GETDATE()
                WHERE UserID=@id",
                new { body.FullName, body.DepartmentID, body.UnitID, body.LineID, body.Title, body.PositionNumber, id });
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.UserID = @id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/users/:id
        [HttpDelete, Route("api/users/{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using var con = _db.CreateConnection();
            await con.ExecuteAsync("DELETE FROM tbl_weekly_report_Users WHERE UserID=@id", new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
