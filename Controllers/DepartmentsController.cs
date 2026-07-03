using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EmployeeInfo;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/departments")]
    public class DepartmentsController : ApiController
    {
        private readonly EmployeeInfoService _employeeInfoService = new EmployeeInfoService();

        // GET /api/departments?unitId=&lineId=
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(int? unitId = null, int? lineId = null)
        {
            var employee = _employeeInfoService.GetEmployeeInfo("UMIT");
            if (employee == null || (unitId.HasValue && employee.UnitId != unitId.Value) || (lineId.HasValue && employee.DivisionId != lineId.Value)) return Ok(new Department[0]);
            return Ok(new[] { new Department { DepartmentID = employee.DepartmentId, DepartmentName = employee.DepartmentName, UnitID = employee.UnitId, UnitName = employee.UnitName, LineID = employee.DivisionId, LineName = employee.DivisionName } });
        }

        // GET /api/departments/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var employee = _employeeInfoService.GetEmployeeInfo("UMIT");
            var row = employee == null || employee.DepartmentId != id
                ? null
                : new Department { DepartmentID = employee.DepartmentId, DepartmentName = employee.DepartmentName, UnitID = employee.UnitId, UnitName = employee.UnitName, LineID = employee.DivisionId, LineName = employee.DivisionName };
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/departments
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateDepartmentRequest body)
        {
            if (body == null || body.UnitID == 0 || string.IsNullOrWhiteSpace(body.DepartmentName))
                return BadRequest("UnitID and DepartmentName are required");
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT /api/departments/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] CreateDepartmentRequest body)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE /api/departments/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
