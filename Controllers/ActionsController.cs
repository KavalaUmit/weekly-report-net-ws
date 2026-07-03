using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/actions")]
    public class ActionsController : ApiController
    {
        private readonly IDbConnectionFactory _db;
        public ActionsController(IDbConnectionFactory db) { _db = db; }

        private const string ActionSelect = @"
            SELECT a.*, w.WeekNumber, w.Year, t.TypeName, t.Header AS TypeHeader, t.IncludeDate, t.SortOrder AS TypeSortOrder, s.StatusKey, s.StatusLabel, s.ColorHex, s.BgColorHex
            FROM tbl_weekly_report_Actions a
            JOIN tbl_weekly_report_Weeks       w  ON w.WeekID      = a.WeekID
            JOIN tbl_weekly_report_ActionTypes t  ON t.TypeID      = a.TypeID
            LEFT JOIN tbl_weekly_report_ActionStatuses s  ON s.StatusID    = a.StatusID";

        // GET /api/actions?userId=&weekId=&weekNumber=&year=&statusId=&lineId=&unitId=
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(
            int? userId = null, int? weekId = null, byte? weekNumber = null,
            short? year = null, int? statusId = null, int? lineId = null, int? unitId = null)
        {
            var conditions = new List<string> { "a.IsDeleted=0" };
            var parameters = new DynamicParameters();

            if (userId.HasValue)     { conditions.Add("a.UserID=@userId");        parameters.Add("userId",     userId); }
            if (weekId.HasValue)     { conditions.Add("a.WeekID=@weekId");        parameters.Add("weekId",     weekId); }
            if (weekNumber.HasValue) { conditions.Add("w.WeekNumber=@weekNumber"); parameters.Add("weekNumber", weekNumber); }
            if (year.HasValue)       { conditions.Add("w.Year=@year");            parameters.Add("year",       year); }
            if (statusId.HasValue)   { conditions.Add("a.StatusID=@statusId");    parameters.Add("statusId",   statusId); }
            if (lineId.HasValue)  { conditions.Add("a.LineID=@lineId");   parameters.Add("lineId",  lineId); }
            if (unitId.HasValue)  { conditions.Add("a.UnitID=@unitId");   parameters.Add("unitId",  unitId); }

            // All entries in 'conditions' are hardcoded column predicates (e.g. "a.UserID=@userId").
            // All values are bound via Dapper DynamicParameters. No user input reaches the SQL string. // NOSONAR
            var where = "WHERE " + string.Join(" AND ", conditions);
            var sql = $"{ActionSelect} {where} ORDER BY a.ActionDate, a.ActionID"; // NOSONAR

            using var con = _db.CreateConnection();
            var rows = await con.QueryAsync<Models.Action>(sql, parameters);
            foreach (var row in rows) row.FullName = row.UserFullName;
            return Ok(rows);
        }

        // GET /api/actions/:id
        [HttpGet, Route("{id:long}")]
        public async Task<IHttpActionResult> GetById(long id)
        {
            using var con = _db.CreateConnection();
            var action = await con.QueryFirstOrDefaultAsync<Models.Action>(
                ActionSelect + " WHERE a.ActionID=@id AND a.IsDeleted=0", new { id });
            if (action == null) return NotFound();
            action.FullName = action.UserFullName;

            var items = await con.QueryAsync<ActionItem>(
                "SELECT * FROM tbl_weekly_report_ActionItems WHERE ActionID=@id ORDER BY SortOrder", new { id });
            action.ActionItems = items.AsList();
            return Ok(action);
        }


                // POST /api/actions
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateActionRequest body)
        {
            if (body == null || body.UserID == 0 || body.WeekID == 0 || body.TypeID == 0
                || string.IsNullOrWhiteSpace(body.ActionDate))
                return BadRequest("UserID, WeekID, TypeID and ActionDate are required");

            using var con = (System.Data.Common.DbConnection)_db.CreateConnection();
            await con.OpenAsync();
            using var tx = con.BeginTransaction();
            try
            {
                var actionId = await con.ExecuteScalarAsync<long>(@"
                    INSERT INTO tbl_weekly_report_Actions (
                        UserID, WeekID, TypeID, ActionDate, StatusID, LineID, UnitID, DepartmentID, WindowsUser, UserFullName,
                        Project, GainType, UpdatedEffort, EffortGain, TTMGain)
                    OUTPUT INSERTED.ActionID
                    VALUES (
                        @UserID, @WeekID, @TypeID, @ActionDate, @StatusID, @LineID, @UnitID, @DepartmentID, @WindowsUser, @UserFullName,
                        @Project, @GainType, @UpdatedEffort, @EffortGain, @TTMGain)",
                    new { body.UserID, body.WeekID, body.TypeID, body.ActionDate, body.StatusID, body.LineID, body.UnitID, body.DepartmentID, body.WindowsUser, body.UserFullName, body.Project, body.GainType, body.UpdatedEffort, body.EffortGain, body.TTMGain }, tx);

                var items = body.actionItems ?? new List<ActionItemInput>();
                for (int i = 0; i < items.Count; i++)
                {
                    await con.ExecuteAsync(@"
                        INSERT INTO tbl_weekly_report_ActionItems (ActionID, SortOrder, ItemType, ItemValue)
                        VALUES (@ActionID, @SortOrder, @ItemType, @ItemValue)",
                        new { ActionID = actionId, SortOrder = (byte)i, ItemType = items[i].type ?? "text", ItemValue = items[i].value ?? "" },
                        tx);
                }

                tx.Commit();
                return Content(HttpStatusCode.Created, new { ActionID = actionId });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // PUT /api/actions/:id
        [HttpPut, Route("{id:long}")]
        public async Task<IHttpActionResult> Update(long id, [FromBody] UpdateActionRequest body)
        {
            using var con = (System.Data.Common.DbConnection)_db.CreateConnection();
            await con.OpenAsync();
            using var tx = con.BeginTransaction();
            try
            {
                await con.ExecuteAsync(@"
                    UPDATE tbl_weekly_report_Actions
                    SET WeekID=COALESCE(@WeekID,WeekID), TypeID=COALESCE(@TypeID,TypeID),
                        ActionDate=COALESCE(@ActionDate,ActionDate), StatusID=COALESCE(@StatusID,StatusID),
                        LineID=COALESCE(@LineID,LineID), UnitID=COALESCE(@UnitID,UnitID), DepartmentID=COALESCE(@DepartmentID,DepartmentID),
                        WindowsUser=COALESCE(@WindowsUser,WindowsUser), UserFullName=COALESCE(@UserFullName,UserFullName),
                        Project=COALESCE(@Project,Project), GainType=COALESCE(@GainType,GainType),
                        UpdatedEffort=COALESCE(@UpdatedEffort,UpdatedEffort), EffortGain=COALESCE(@EffortGain,EffortGain), TTMGain=COALESCE(@TTMGain,TTMGain),
                        UpdatedAt=GETDATE()
                    WHERE ActionID=@id AND IsDeleted=0",
                    new { body.WeekID, body.TypeID, body.ActionDate, body.StatusID, body.LineID, body.UnitID, body.DepartmentID, body.WindowsUser, body.UserFullName, body.Project, body.GainType, body.UpdatedEffort, body.EffortGain, body.TTMGain, id }, tx);

                if (body.actionItems != null)
                {
                    await con.ExecuteAsync(
                        "DELETE FROM tbl_weekly_report_ActionItems WHERE ActionID=@id", new { id }, tx);

                    for (int i = 0; i < body.actionItems.Count; i++)
                    {
                        await con.ExecuteAsync(@"
                            INSERT INTO tbl_weekly_report_ActionItems (ActionID, SortOrder, ItemType, ItemValue)
                            VALUES (@ActionID, @SortOrder, @ItemType, @ItemValue)",
                            new { ActionID = id, SortOrder = (byte)i, ItemType = body.actionItems[i].type ?? "text", ItemValue = body.actionItems[i].value ?? "" },
                            tx);
                    }
                }

                tx.Commit();
                return Ok(new { ActionID = id });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // PATCH /api/actions/:id/status
        [HttpPatch, Route("{id:long}/status")]
        public async Task<IHttpActionResult> PatchStatus(long id, [FromBody] PatchActionStatusRequest body)
        {
            using var con = (System.Data.Common.DbConnection)_db.CreateConnection();
            await con.OpenAsync();
            using var tx = con.BeginTransaction();
            try
            {
                await con.ExecuteAsync(@"
                    UPDATE tbl_weekly_report_Actions
                    SET StatusID=@StatusID, UpdatedAt=GETDATE()
                    WHERE ActionID=@id",
                    new { body.StatusID, id }, tx);

                if (body.StatusID.HasValue)
                {
                    await con.ExecuteAsync(@"
                        INSERT INTO tbl_weekly_report_ActionStatusHistory (ActionID, StatusID, ChangedBy)
                        VALUES (@id, @StatusID, @ChangedBy)",
                        new { id, body.StatusID, body.ChangedBy }, tx);
                }

                tx.Commit();
                return Ok(new { ActionID = id, body.StatusID });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // DELETE /api/actions/:id (soft delete)
        [HttpDelete, Route("{id:long}")]
        public async Task<IHttpActionResult> Delete(long id)
        {
            using var con = _db.CreateConnection();
            await con.ExecuteAsync(
                "UPDATE tbl_weekly_report_Actions SET IsDeleted=1, UpdatedAt=GETDATE() WHERE ActionID=@id",
                new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
