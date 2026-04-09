using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/units")]
    public class UnitsController : ApiController
    {
        // GET /api/units?lineId=
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(int? lineId = null)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var sql = @"SELECT u.*, l.LineName
                FROM tbl_weekly_report_Units u
                JOIN tbl_weekly_report_Lines l ON l.LineID = u.LineID";
            if (lineId.HasValue) sql += " WHERE u.LineID = @lineId";
            sql += " ORDER BY l.LineName, u.UnitName";
            var rows = await con.QueryAsync<Unit>(sql, new { lineId });
            return Ok(rows);
        }

        // GET /api/units/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<Unit>(@"
                SELECT u.*, l.LineName
                FROM tbl_weekly_report_Units u
                JOIN tbl_weekly_report_Lines l ON l.LineID = u.LineID
                WHERE u.UnitID = @id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/units
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateUnitRequest body)
        {
            if (body == null || body.LineID == 0 || string.IsNullOrWhiteSpace(body.UnitName))
                return BadRequest("LineID and UnitName are required");
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstAsync<Unit>(
                "INSERT INTO tbl_weekly_report_Units (LineID, UnitName) OUTPUT INSERTED.* VALUES (@LineID, @UnitName)",
                new { body.LineID, body.UnitName });
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/units/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] CreateUnitRequest body)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<Unit>(
                "UPDATE tbl_weekly_report_Units SET LineID=@LineID, UnitName=@UnitName OUTPUT INSERTED.* WHERE UnitID=@id",
                new { body.LineID, body.UnitName, id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/units/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            await con.ExecuteAsync("DELETE FROM tbl_weekly_report_Units WHERE UnitID=@id", new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
