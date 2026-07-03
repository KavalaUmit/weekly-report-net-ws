using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EmployeeInfo;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/units")]
    public class UnitsController : ApiController
    {
        private readonly EmployeeInfoService _employeeInfoService = new EmployeeInfoService();

        // GET /api/units?lineId=
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll(int? lineId = null)
        {
            var employee = _employeeInfoService.GetEmployeeInfo("UMIT");
            if (employee == null || (lineId.HasValue && employee.DivisionId != lineId.Value)) return Ok(new Unit[0]);
            return Ok(new[] { new Unit { UnitID = employee.UnitId, UnitName = employee.UnitName, LineID = employee.DivisionId, LineName = employee.DivisionName } });
        }

        // GET /api/units/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var employee = _employeeInfoService.GetEmployeeInfo("UMIT");
            var row = employee == null || employee.UnitId != id
                ? null
                : new Unit { UnitID = employee.UnitId, UnitName = employee.UnitName, LineID = employee.DivisionId, LineName = employee.DivisionName };
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/units
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateUnitRequest body)
        {
            if (body == null || body.LineID == 0 || string.IsNullOrWhiteSpace(body.UnitName))
                return BadRequest("LineID and UnitName are required");
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT /api/units/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] CreateUnitRequest body)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE /api/units/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
