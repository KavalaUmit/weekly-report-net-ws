using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using WeeklyReportWS.Controllers;
using Xunit;

namespace WeeklyReportWS.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly HealthController _controller;

        public HealthControllerTests()
        {
            _controller = new HealthController();
            _controller.Request = new HttpRequestMessage();
            _controller.Configuration = new HttpConfiguration();
        }

        [Fact]
        public void Get_ReturnsOk()
        {
            var result = _controller.Get();

            Assert.NotNull(result);
            Assert.Contains("OkNegotiatedContentResult", result.GetType().Name);
        }

        [Fact]
        public void Get_ResponseContainsStatusOk()
        {
            var result = _controller.Get();
            dynamic content = result.GetType().GetProperty("Content")?.GetValue(result);

            Assert.NotNull(content);
            var statusProp = ((object)content).GetType().GetProperty("status");
            Assert.NotNull(statusProp);
            Assert.Equal("ok", statusProp.GetValue(content));
        }

        [Fact]
        public void Get_ResponseContainsTimestamp()
        {
            var result = _controller.Get();
            dynamic content = result.GetType().GetProperty("Content")?.GetValue(result);

            Assert.NotNull(content);
            var tsProp = ((object)content).GetType().GetProperty("timestamp");
            Assert.NotNull(tsProp);
            Assert.IsType<DateTime>(tsProp.GetValue(content));
        }
    }
}
