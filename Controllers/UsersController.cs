using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EmployeeInfo;
using EmployeeInfo.Models;
using WeeklyReportWS.Models;

namespace WeeklyReportWS.Controllers
{
    public class UsersController : ApiController
    {
        private readonly EmployeeInfoService _employeeInfoService = new EmployeeInfoService();

        // GET /api/users
        [HttpGet, Route("api/users")]
        public async Task<IHttpActionResult> GetAll()
        {
            var user = ToUser(_employeeInfoService.GetEmployeeInfo("UMIT"));
            return Ok(user == null ? new User[0] : new[] { user });
        }

        // GET /user/getuserdata?windowName=  (legacy)
        [HttpGet, Route("user/getuserdata")]
        public async Task<IHttpActionResult> GetUserData(string? windowsName = null)
        {
            if (string.IsNullOrWhiteSpace(windowsName))
                return BadRequest("windowsName is required");
            var row = ToUser(_employeeInfoService.GetEmployeeInfo(windowsName));
            if (row == null) return NotFound();
            return Ok(row);
        }

        // GET /api/users/windowname/:windowName
        [HttpGet, Route("api/users/windowsname/{windowsName}")]
        public async Task<IHttpActionResult> GetByWindowsName(string windowsName)
        {
            var row = ToUser(_employeeInfoService.GetEmployeeInfo(windowsName));
            if (row == null) return NotFound();
            return Ok(row);
        }

        // GET /api/users/me  – resolved from Windows Authentication (User.Identity.Name)
        [Authorize]
        [HttpGet, Route("api/users/me")]
        public async Task<IHttpActionResult> GetMe()
        {
            var rawName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(rawName))
                return Unauthorized();

            // IIS returns DOMAIN\username – try both forms against the DB
            var shortName = rawName.Contains("\\") ? rawName.Split('\\')[1] : rawName;

            // Uppercase with en-US/Invariant rules to avoid Turkish i→İ conflicts
            var rawUpper   = rawName.ToUpper(CultureInfo.InvariantCulture);
            var shortUpper = shortName.ToUpper(CultureInfo.InvariantCulture);

            var row = ToUser(_employeeInfoService.GetEmployeeInfo(rawUpper))
                ?? ToUser(_employeeInfoService.GetEmployeeInfo(shortUpper));
            if (row == null)
                return Content(HttpStatusCode.NotFound,
                    new { WindowsName = rawName, ShortName = shortName });
            return Ok(row);
        }

        private static User? ToUser(EmployeeSearchResult? employee)
        {
            if (employee == null) return null;

            return new User
            {
                UserID = employee.UserId,
                WindowsName = employee.WindowsUsername,
                FullName = employee.Name,
                Title = employee.Title,
                PositionNumber = employee.TitleLevel > byte.MaxValue ? byte.MaxValue : (byte?)employee.TitleLevel,
                DepartmentID = employee.DepartmentId,
                DepartmentName = employee.DepartmentName,
                UnitID = employee.UnitId,
                UnitName = employee.UnitName,
                LineID = employee.DivisionId,
                LineName = employee.DivisionName
            };
        }

        // GET /api/users/:id
        [HttpGet, Route("api/users/{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var row = ToUser(_employeeInfoService.GetEmployeeInfo("UMIT"));
            if (row?.UserID != id) return NotFound();
            if (row == null) return NotFound();
            return Ok(row);
        }

        // POST /api/users
        [HttpPost, Route("api/users")]
        public async Task<IHttpActionResult> Create([FromBody] CreateUserRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.WindowsName) || string.IsNullOrWhiteSpace(body.FullName))
                return BadRequest("WindowsName and FullName are required");
            var row = ToUser(_employeeInfoService.GetEmployeeInfo(body.WindowsName));
            if (row == null) return NotFound();
            return Content(HttpStatusCode.Created, row);
        }

        // PUT /api/users/:id
        [HttpPut, Route("api/users/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] UpdateUserRequest body)
        {
            var row = ToUser(_employeeInfoService.GetEmployeeInfo("UMIT"));
            if (row?.UserID != id) return NotFound();
            if (row == null) return NotFound();
            return Ok(row);
        }

        // DELETE /api/users/:id
        [HttpDelete, Route("api/users/{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
