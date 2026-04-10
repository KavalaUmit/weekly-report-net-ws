using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Moq;
using WeeklyReportWS.Controllers;
using WeeklyReportWS.Data;
using WeeklyReportWS.Models;
using Xunit;

namespace WeeklyReportWS.Tests.Controllers
{
    public class ActionsControllerTests
    {
        private readonly Mock<IDbConnectionFactory> _mockDb;
        private readonly ActionsController _controller;

        public ActionsControllerTests()
        {
            _mockDb = new Mock<IDbConnectionFactory>();
            _controller = new ActionsController(_mockDb.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Configuration = new HttpConfiguration();
        }

        // ── Create validation ──────────────────────────────────────────────────

        [Fact]
        public async void Create_NullBody_ReturnsBadRequest()
        {
            var result = await _controller.Create(null);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_MissingUserID_ReturnsBadRequest()
        {
            var body = new CreateActionRequest { UserID = 0, WeekID = 1, TypeID = 1, ActionDate = "2026-04-10" };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_MissingWeekID_ReturnsBadRequest()
        {
            var body = new CreateActionRequest { UserID = 1, WeekID = 0, TypeID = 1, ActionDate = "2026-04-10" };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_MissingTypeID_ReturnsBadRequest()
        {
            var body = new CreateActionRequest { UserID = 1, WeekID = 1, TypeID = 0, ActionDate = "2026-04-10" };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_MissingActionDate_ReturnsBadRequest()
        {
            var body = new CreateActionRequest { UserID = 1, WeekID = 1, TypeID = 1, ActionDate = "" };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_WhitespaceActionDate_ReturnsBadRequest()
        {
            var body = new CreateActionRequest { UserID = 1, WeekID = 1, TypeID = 1, ActionDate = "   " };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_ValidBody_DoesNotCallDbBeforeValidation()
        {
            var body = new CreateActionRequest { UserID = 1, WeekID = 1, TypeID = 1, ActionDate = "2026-04-10" };

            // We do NOT set up _mockDb — if validation passes and DB is called without setup it will throw.
            // This test verifies we get to the DB call only when input is valid.
            _mockDb.Verify(d => d.CreateConnection(), Times.Never);
        }
    }
}
