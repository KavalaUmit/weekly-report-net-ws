using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using WeeklyReportWS.Data;
using WeeklyReportWS.Filters;
using WeeklyReportWS.Infrastructure;

[assembly: OwinStartup(typeof(WeeklyReportWS.Startup))]

namespace WeeklyReportWS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(BuildCorsOptions());

            var config = new HttpConfiguration();
            config.DependencyResolver = new SimpleDependencyResolver(new SqlDbConnectionFactory());
            config.MapHttpAttributeRoutes();

            config.Filters.Add(new GlobalExceptionFilter());

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                NullValueHandling = NullValueHandling.Include
            };

            config.Routes.MapHttpRoute(
                name: "Health",
                routeTemplate: "health",
                defaults: new { controller = "Health", action = "Get" }
            );

            app.UseWebApi(config);
        }

        private static CorsOptions BuildCorsOptions()
        {
            var originsConfig = ConfigurationManager.AppSettings["AllowedOrigins"] ?? string.Empty;
            var origins = originsConfig
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Trim())
                .Where(o => !string.IsNullOrEmpty(o))
                .ToArray();

            return new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = req =>
                    {
                        var policy = new CorsPolicy
                        {
                            AllowAnyHeader = true,
                            AllowAnyMethod = true,
                            SupportsCredentials = false
                        };
                        if (origins.Length == 0)
                            policy.AllowAnyOrigin = true;
                        else
                            foreach (var o in origins)
                                policy.Origins.Add(o);
                        return Task.FromResult(policy);
                    }
                }
            };
        }
    }
}
