using System;
using System.Configuration;
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

        [HttpGet]
        [Route("health/diagnostics")]
        public IHttpActionResult Diagnostics()
        {
            var connStrEntry = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            var envVar = Environment.GetEnvironmentVariable("WEEKLY_REPORT_CONNECTION_STRING");
            var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            return Ok(new
            {
                configFile,
                connectionStringFound = connStrEntry != null,
                connectionStringLength = connStrEntry?.ConnectionString?.Length ?? 0,
                envVarFound = !string.IsNullOrEmpty(envVar),
                envVarLength = envVar?.Length ?? 0
            });
        }
    }
}
