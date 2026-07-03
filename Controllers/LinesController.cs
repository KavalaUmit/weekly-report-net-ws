using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EmployeeInfo;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/lines")]
    public class LinesController : ApiController
    {
        private readonly EmployeeInfoService _employeeInfoService = new EmployeeInfoService();

        // GET /api/lines
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            var employee = _employeeInfoService.GetEmployeeInfo("UMIT");
            return Ok(employee == null
                ? new Line[0]
                : new[] { new Line { LineID = employee.DivisionId, LineName = employee.DivisionName } });
        }

        // GET /api/lines/hierarchy
        [HttpGet, Route("hierarchy")]
        public async Task<IHttpActionResult> GetHierarchy()
        {
            var hierarchy = _employeeInfoService.GetDivisionHierarchy();
            return Ok(hierarchy);
        }

        // GET /api/lines/:id
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var employee = _employeeInfoService.GetEmployeeInfo("UMIT");
            var row = employee == null || employee.DivisionId != id
                ? null
                : new Line { LineID = employee.DivisionId, LineName = employee.DivisionName };
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/lines
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateLineRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.LineName))
                return BadRequest("LineName is required");
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT /api/lines/:id
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] CreateLineRequest body)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE /api/lines/:id
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
