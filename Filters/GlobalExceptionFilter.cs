using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace WeeklyReportWS.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new { error = "An internal server error occurred." }
            );
        }
    }
}
