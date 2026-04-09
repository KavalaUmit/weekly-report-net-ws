using System;
using System.Web.Http;

namespace WeeklyReportWS.Controllers
{
    public class HealthController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
        }
    }
}
