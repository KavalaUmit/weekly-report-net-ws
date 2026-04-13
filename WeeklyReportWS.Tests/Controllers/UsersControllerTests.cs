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
    public class UsersControllerTests
    {
        private readonly Mock<IDbConnectionFactory> _mockDb;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockDb = new Mock<IDbConnectionFactory>();
            _controller = new UsersController(_mockDb.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Configuration = new HttpConfiguration();
        }

        // ── GetUserData validation ─────────────────────────────────────────────

        [Fact]
        public async void GetUserData_NullWindowsName_ReturnsBadRequest()
        {
            var result = await _controller.GetUserData(null);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void GetUserData_EmptyWindowsName_ReturnsBadRequest()
        {
            var result = await _controller.GetUserData("");

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void GetUserData_WhitespaceWindowsName_ReturnsBadRequest()
        {
            var result = await _controller.GetUserData("   ");

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        // ── Create validation ──────────────────────────────────────────────────

        [Fact]
        public async void Create_NullBody_ReturnsBadRequest()
        {
            var result = await _controller.Create(null);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_MissingWindowsName_ReturnsBadRequest()
        {
            var body = new CreateUserRequest { WindowsName = "", FullName = "John Doe" };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_WhitespaceWindowsName_ReturnsBadRequest()
        {
            var body = new CreateUserRequest { WindowsName = "   ", FullName = "John Doe" };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_MissingFullName_ReturnsBadRequest()
        {
            var body = new CreateUserRequest { WindowsName = "DOMAIN\\jdoe", FullName = "" };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_NullFullName_ReturnsBadRequest()
        {
            var body = new CreateUserRequest { WindowsName = "DOMAIN\\jdoe", FullName = null };

            var result = await _controller.Create(body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Create_ValidBody_DbIsNotCalledUntilAfterValidation()
        {
            var body = new CreateUserRequest { WindowsName = "DOMAIN\\jdoe", FullName = "John Doe" };

            _mockDb.Verify(d => d.CreateConnection(), Times.Never);
        }
    }
}
