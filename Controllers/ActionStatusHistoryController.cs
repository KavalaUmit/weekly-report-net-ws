using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/actions")]
    public class ActionStatusHistoryController : ApiController
    {
        private readonly IDbConnectionFactory _db;
        public ActionStatusHistoryController(IDbConnectionFactory db) { _db = db; }

        // GET /api/actions/:actionId/status-history
        [HttpGet, Route("{actionId:long}/status-history")]
        public async Task<IHttpActionResult> GetHistory(long actionId)
        {
            using var con = _db.CreateConnection();
            var rows = await con.QueryAsync<ActionStatusHistory>(@"
                SELECT h.*, s.StatusKey, s.StatusLabel, s.ColorHex, u.FullName AS ChangedByName
                FROM tbl_weekly_report_ActionStatusHistory h
                LEFT JOIN tbl_weekly_report_ActionStatuses s ON s.StatusID = h.StatusID
                JOIN tbl_weekly_report_Users u ON u.UserID = h.ChangedBy
                WHERE h.ActionID = @actionId
                ORDER BY h.ChangedAt DESC",
                new { actionId });
            return Ok(rows);
        }

        // POST /api/actions/:actionId/status-history
        [HttpPost, Route("{actionId:long}/status-history")]
        public async Task<IHttpActionResult> AddHistory(long actionId, [FromBody] CreateStatusHistoryRequest body)
        {
            if (body.ChangedBy == 0)
                return BadRequest("ChangedBy is required");
            using var con = _db.CreateConnection();
            var row = await con.QueryFirstAsync<ActionStatusHistory>(@"
                INSERT INTO tbl_weekly_report_ActionStatusHistory (ActionID, StatusID, ChangedBy)
                OUTPUT INSERTED.*
                VALUES (@actionId, @StatusID, @ChangedBy)",
                new { actionId, body.StatusID, body.ChangedBy });
            return Content(HttpStatusCode.Created, row);
        }

        // DELETE /api/actions/:actionId/status-history/:historyId
        [HttpDelete, Route("{actionId:long}/status-history/{historyId:long}")]
        public async Task<IHttpActionResult> DeleteHistory(long actionId, long historyId)
        {
            using var con = _db.CreateConnection();
            await con.ExecuteAsync(
                "DELETE FROM tbl_weekly_report_ActionStatusHistory WHERE HistoryID=@historyId AND ActionID=@actionId",
                new { historyId, actionId });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
