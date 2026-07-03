using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace WeeklyReportWS.Controllers
{
    [RoutePrefix("api/config")]
    public class ConfigController : ApiController
    {
        // GET /api/config/gain-types
        [HttpGet, Route("gain-types")]
        public IHttpActionResult GetGainTypes()
        {
            var raw = ConfigurationManager.AppSettings["Action.GainTypeOptions"];
            var list = string.IsNullOrWhiteSpace(raw)
                ? new[] { "Maliyet-Kalite" }
                : raw.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(s => s.Trim())
                     .Where(s => s.Length > 0)
                     .ToArray();
            return Ok(list);
        }
    }
}
