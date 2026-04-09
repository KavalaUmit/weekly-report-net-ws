using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/action-types")]
    public class ActionTypesController : ApiController
    {
        // GET /api/action-types
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            using var con = DbConnectionFactory.CreateConnection();
            var rows = await con.QueryAsync<ActionType>(
                "SELECT * FROM tbl_weekly_report_ActionTypes WHERE IsActive=1 ORDER BY SortOrder");
            return Ok(rows);
        }

        // GET /api/action-types/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<ActionType>(
                "SELECT * FROM tbl_weekly_report_ActionTypes WHERE TypeID=@id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/action-types
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateActionTypeRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.TypeName))
                return BadRequest("TypeName is required");
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstAsync<ActionType>(
                "INSERT INTO tbl_weekly_report_ActionTypes (TypeName, SortOrder) OUTPUT INSERTED.* VALUES (@TypeName, @SortOrder)",
                new { body.TypeName, body.SortOrder });
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/action-types/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] UpdateActionTypeRequest body)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<ActionType>(@"
                UPDATE tbl_weekly_report_ActionTypes
                SET TypeName=@TypeName, SortOrder=@SortOrder, IsActive=@IsActive
                OUTPUT INSERTED.*
                WHERE TypeID=@id",
                new { body.TypeName, body.SortOrder, body.IsActive, id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/action-types/:id (soft delete)
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            await con.ExecuteAsync(
                "UPDATE tbl_weekly_report_ActionTypes SET IsActive=0 WHERE TypeID=@id", new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
