using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/lines")]
    public class LinesController : ApiController
    {
        private readonly IDbConnectionFactory _db;
        public LinesController(IDbConnectionFactory db) { _db = db; }

        // GET /api/lines
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            using var con = _db.CreateConnection();
            var rows = await con.QueryAsync<Line>("SELECT * FROM tbl_weekly_report_Lines ORDER BY LineName");
            return Ok(rows);
        }

        // GET /api/lines/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<Line>(
                "SELECT * FROM tbl_weekly_report_Lines WHERE LineID = @id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/lines
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateLineRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.LineName))
                return BadRequest("LineName is required");
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstAsync<Line>(
                "INSERT INTO tbl_weekly_report_Lines (LineName) OUTPUT INSERTED.* VALUES (@LineName)",
                new { body.LineName });
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/lines/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] CreateLineRequest body)
        {
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<Line>(
                "UPDATE tbl_weekly_report_Lines SET LineName=@LineName OUTPUT INSERTED.* WHERE LineID=@id",
                new { body.LineName, id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/lines/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using var con = _db.CreateConnection();
            await con.ExecuteAsync("DELETE FROM tbl_weekly_report_Lines WHERE LineID=@id", new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
