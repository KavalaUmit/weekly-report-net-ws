using System;
using System.Collections.Generic;
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
using Swashbuckle.Application;
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
            app.Use((context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    return Task.CompletedTask;
                }

                return next();
            });

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

            var enableSwagger = string.Equals(
                ConfigurationManager.AppSettings["EnableSwagger"], "true",
                StringComparison.OrdinalIgnoreCase);
            if (enableSwagger)
            {
                config.EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Weekly Report API");
                    c.PrettyPrint();
                }).EnableSwaggerUi();
            }

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
                        var allowed = new List<string> { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
                        var allowedHeaders = new List<string> { "Content-Type", "Authorization", "X-Requested-With" };
                        var policy = new CorsPolicy { SupportsCredentials = true };
                        foreach (var m in allowed)       policy.Methods.Add(m);
                        foreach (var h in allowedHeaders) policy.Headers.Add(h);
                        // SupportsCredentials=true requires explicit origins (no wildcard)
                        if (origins.Length > 0)
                            foreach (var o in origins)
                                policy.Origins.Add(o);
                        else
                            policy.AllowAnyOrigin = true;
                        return Task.FromResult(policy);
                    }
                }
            };
        }
    }
}
