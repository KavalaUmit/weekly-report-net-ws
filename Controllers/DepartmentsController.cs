using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/departments")]
    public class DepartmentsController : ApiController
    {
        // GET /api/departments?unitId=&lineId=
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(int? unitId = null, int? lineId = null)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var conditions = new List<string>();
            if (unitId.HasValue) conditions.Add("d.UnitID = @unitId");
            if (lineId.HasValue) conditions.Add("u.LineID = @lineId");
            var where = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
            var sql = $@"SELECT d.*, u.UnitName, u.LineID, l.LineName
                FROM tbl_weekly_report_Departments d
                JOIN tbl_weekly_report_Units u ON u.UnitID = d.UnitID
                JOIN tbl_weekly_report_Lines l ON l.LineID = u.LineID
                {where}
                ORDER BY l.LineName, u.UnitName, d.DepartmentName";
            var rows = await con.QueryAsync<Department>(sql, new { unitId, lineId });
            return Ok(rows);
        }

        // GET /api/departments/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<Department>(@"
                SELECT d.*, u.UnitName, u.LineID, l.LineName
                FROM tbl_weekly_report_Departments d
                JOIN tbl_weekly_report_Units u ON u.UnitID = d.UnitID
                JOIN tbl_weekly_report_Lines l ON l.LineID = u.LineID
                WHERE d.DepartmentID = @id", new { id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/departments
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateDepartmentRequest body)
        {
            if (body == null || body.UnitID == 0 || string.IsNullOrWhiteSpace(body.DepartmentName))
                return BadRequest("UnitID and DepartmentName are required");
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstAsync<Department>(
                "INSERT INTO tbl_weekly_report_Departments (UnitID, DepartmentName) OUTPUT INSERTED.* VALUES (@UnitID, @DepartmentName)",
                new { body.UnitID, body.DepartmentName });
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/departments/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] CreateDepartmentRequest body)
        {
            using var con = DbConnectionFactory.CreateConnection();
            var row = await con.QueryFirstOrDefaultAsync<Department>(
                "UPDATE tbl_weekly_report_Departments SET UnitID=@UnitID, DepartmentName=@DepartmentName OUTPUT INSERTED.* WHERE DepartmentID=@id",
                new { body.UnitID, body.DepartmentName, id });
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/departments/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using var con = DbConnectionFactory.CreateConnection();
            await con.ExecuteAsync("DELETE FROM tbl_weekly_report_Departments WHERE DepartmentID=@id", new { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
