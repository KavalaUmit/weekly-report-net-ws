using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/actions")]
    public class ActionItemsController : ApiController
    {
        // GET /api/actions/:actionId/items
        [HttpGet, Route("{actionId:long}/items")]
        public async Task<IHttpActionResult> GetItems(long actionId)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var rows = await con.QueryAsync<ActionItem>(
                "SELECT * FROM tbl_weekly_report_ActionItems WHERE ActionID=@actionId ORDER BY SortOrder",
                new { actionId });
            return Ok(rows);
        }

        // POST /api/actions/:actionId/items
        [HttpPost, Route("{actionId:long}/items")]
        public async Task<IHttpActionResult> AddItem(long actionId, [FromBody] CreateActionItemRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.ItemType) || body.ItemValue == null)
                return BadRequest("ItemType and ItemValue are required");
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstAsync<ActionItem>(@"
                INSERT INTO tbl_weekly_report_ActionItems (ActionID, SortOrder, ItemType, ItemValue)
                OUTPUT INSERTED.*
                VALUES (@actionId, @SortOrder, @ItemType, @ItemValue)",
                new { actionId, body.SortOrder, body.ItemType, body.ItemValue });
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/actions/:actionId/items/:itemId
        [HttpPut, Route("{actionId:long}/items/{itemId:long}")]
        public async Task<IHttpActionResult> UpdateItem(long actionId, long itemId, [FromBody] UpdateActionItemRequest body)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<ActionItem>(@"
                UPDATE tbl_weekly_report_ActionItems
                SET SortOrder=@SortOrder, ItemType=@ItemType, ItemValue=@ItemValue
                OUTPUT INSERTED.*
                WHERE ItemID=@itemId AND ActionID=@actionId",
                new { body.SortOrder, body.ItemType, body.ItemValue, itemId, actionId });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/actions/:actionId/items/:itemId
        [HttpDelete, Route("{actionId:long}/items/{itemId:long}")]
        public async Task<IHttpActionResult> DeleteItem(long actionId, long itemId)
        {
            using var con = DbConnectionFactory.CreateConnection();
            await con.ExecuteAsync(
                "DELETE FROM tbl_weekly_report_ActionItems WHERE ItemID=@itemId AND ActionID=@actionId",
                new { itemId, actionId });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
