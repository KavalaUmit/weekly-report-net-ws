using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/action-statuses")]
    public class ActionStatusesController : ApiController
    {
        // GET /api/action-statuses
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            using var con = DbConnectionFactory.CreateConnection();
            var rows = await con.QueryAsync<ActionStatus>(
                "SELECT * FROM tbl_weekly_report_ActionStatuses ORDER BY StatusID");
            return Ok(rows);
        }

        // GET /api/action-statuses/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<ActionStatus>(
                "SELECT * FROM tbl_weekly_report_ActionStatuses WHERE StatusID=@id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/action-statuses
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateActionStatusRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.StatusKey) || string.IsNullOrWhiteSpace(body.StatusLabel)
                || string.IsNullOrWhiteSpace(body.ColorHex) || string.IsNullOrWhiteSpace(body.BgColorHex))
                return BadRequest("StatusKey, StatusLabel, ColorHex and BgColorHex are required");
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstAsync<ActionStatus>(@"
                INSERT INTO tbl_weekly_report_ActionStatuses (StatusKey, StatusLabel, ColorHex, BgColorHex)
                OUTPUT INSERTED.*
                VALUES (@StatusKey, @StatusLabel, @ColorHex, @BgColorHex)",
                new { body.StatusKey, body.StatusLabel, body.ColorHex, body.BgColorHex });
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/action-statuses/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] UpdateActionStatusRequest body)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<ActionStatus>(@"
                UPDATE tbl_weekly_report_ActionStatuses
                SET StatusLabel=@StatusLabel, ColorHex=@ColorHex, BgColorHex=@BgColorHex
                OUTPUT INSERTED.*
                WHERE StatusID=@id",
                new { body.StatusLabel, body.ColorHex, body.BgColorHex, id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/action-statuses/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            await con.ExecuteAsync(
                "DELETE FROM tbl_weekly_report_ActionStatuses WHERE StatusID=@id", new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
