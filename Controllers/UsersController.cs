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
            SELECT u.UserID, u.WindowName, u.FullName, u.Title, u.PositionNumber,
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
        public async Task<IHttpActionResult> GetUserData(string? windowName = null)
        {
            if (string.IsNullOrWhiteSpace(windowName))
                return BadRequest("windowName is required");
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.WindowName = @windowName", new { windowName });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // GET /api/users/windowname/:windowName
        [HttpGet, Route("api/users/windowname/{windowName}")]
        public async Task<IHttpActionResult> GetByWindowName(string windowName)
        {
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.WindowName = @windowName", new { windowName });
            if (row == null) return NotFound();
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
            if (body == null || string.IsNullOrWhiteSpace(body.WindowName) || string.IsNullOrWhiteSpace(body.FullName))
                return BadRequest("WindowName and FullName are required");
            using var con = _db.CreateConnection();
            await con.ExecuteAsync(@"
                INSERT INTO tbl_weekly_report_Users (WindowName,FullName,DepartmentID,UnitID,LineID,Title,PositionNumber)
                VALUES (@WindowName,@FullName,@DepartmentID,@UnitID,@LineID,@Title,@PositionNumber)",
                new { body.WindowName, body.FullName, body.DepartmentID, body.UnitID, body.LineID, body.Title, body.PositionNumber });
            var row = await con.QueryFirstOrDefaultAsync<User>(
                UserSelect + " WHERE u.WindowName = @windowName", new { windowName = body.WindowName });
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
