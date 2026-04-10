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
    public class ActionItemsControllerTests
    {
        private readonly Mock<IDbConnectionFactory> _mockDb;
        private readonly ActionItemsController _controller;

        public ActionItemsControllerTests()
        {
            _mockDb = new Mock<IDbConnectionFactory>();
            _controller = new ActionItemsController(_mockDb.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Configuration = new HttpConfiguration();
        }

        [Fact]
        public async void AddItem_NullBody_ReturnsBadRequest()
        {
            var result = await _controller.AddItem(1, null);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void AddItem_NullItemType_ReturnsBadRequest()
        {
            var body = new CreateActionItemRequest { ItemType = null, ItemValue = "some value" };

            var result = await _controller.AddItem(1, body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void AddItem_WhitespaceItemType_ReturnsBadRequest()
        {
            var body = new CreateActionItemRequest { ItemType = "  ", ItemValue = "some value" };

            var result = await _controller.AddItem(1, body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void AddItem_NullItemValue_ReturnsBadRequest()
        {
            var body = new CreateActionItemRequest { ItemType = "text", ItemValue = null };

            var result = await _controller.AddItem(1, body);

            Assert.IsType<BadRequestErrorMessageResult>(result);
        }
    }
}
