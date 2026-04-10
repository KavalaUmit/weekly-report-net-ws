using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/weeks")]
    public class WeeksController : ApiController
    {
        private readonly IDbConnectionFactory _db;
        public WeeksController(IDbConnectionFactory db) { _db = db; }

        // GET /api/weeks?year=
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(short? year = null)
        {
            using var con = _db.CreateConnection();
            var sql = "SELECT * FROM tbl_weekly_report_Weeks";
            if (year.HasValue) sql += " WHERE Year=@year";
            sql += " ORDER BY Year, WeekNumber";
            var rows = await con.QueryAsync<Week>(sql, new { year });
            return Ok(rows);
        }

        // GET /api/weeks/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<Week>(
                "SELECT * FROM tbl_weekly_report_Weeks WHERE WeekID=@id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/weeks
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateWeekRequest body)
        {
            if (body == null || body.WeekNumber == 0 || body.Year == 0)
                return BadRequest("WeekNumber and Year are required");
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstAsync<Week>(
                "INSERT INTO tbl_weekly_report_Weeks (WeekNumber, Year) OUTPUT INSERTED.* VALUES (@WeekNumber, @Year)",
                new { body.WeekNumber, body.Year });
            return Content(HttpStatusCode.Created, row);
        }

        // DELETE /api/weeks/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using var con = _db.CreateConnection();
            await con.ExecuteAsync("DELETE FROM tbl_weekly_report_Weeks WHERE WeekID=@id", new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
