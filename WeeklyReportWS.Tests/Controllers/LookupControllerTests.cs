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
    public class LookupControllerTests
    {
        private readonly Mock<IDbConnectionFactory> _mockDb;

        public LookupControllerTests()
        {
            _mockDb = new Mock<IDbConnectionFactory>();
        }

        private T BuildController<T>(T controller) where T : System.Web.Http.ApiController
        {
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            return controller;
        }

        // ── LinesController ────────────────────────────────────────────────────

        [Fact]
        public async void Lines_Create_NullBody_ReturnsBadRequest()
        {
            var ctrl = BuildController(new LinesController(_mockDb.Object));
            var result = await ctrl.Create(null);
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Lines_Create_EmptyLineName_ReturnsBadRequest()
        {
            var ctrl = BuildController(new LinesController(_mockDb.Object));
            var result = await ctrl.Create(new CreateLineRequest { LineName = "" });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Lines_Create_WhitespaceLineName_ReturnsBadRequest()
        {
            var ctrl = BuildController(new LinesController(_mockDb.Object));
            var result = await ctrl.Create(new CreateLineRequest { LineName = "   " });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        // ── UnitsController ────────────────────────────────────────────────────

        [Fact]
        public async void Units_Create_NullBody_ReturnsBadRequest()
        {
            var ctrl = BuildController(new UnitsController(_mockDb.Object));
            var result = await ctrl.Create(null);
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Units_Create_MissingLineID_ReturnsBadRequest()
        {
            var ctrl = BuildController(new UnitsController(_mockDb.Object));
            var result = await ctrl.Create(new CreateUnitRequest { LineID = 0, UnitName = "Dev" });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Units_Create_MissingUnitName_ReturnsBadRequest()
        {
            var ctrl = BuildController(new UnitsController(_mockDb.Object));
            var result = await ctrl.Create(new CreateUnitRequest { LineID = 1, UnitName = "" });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        // ── DepartmentsController ──────────────────────────────────────────────

        [Fact]
        public async void Departments_Create_NullBody_ReturnsBadRequest()
        {
            var ctrl = BuildController(new DepartmentsController(_mockDb.Object));
            var result = await ctrl.Create(null);
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Departments_Create_MissingUnitID_ReturnsBadRequest()
        {
            var ctrl = BuildController(new DepartmentsController(_mockDb.Object));
            var result = await ctrl.Create(new CreateDepartmentRequest { UnitID = 0, DepartmentName = "Engineering" });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Departments_Create_MissingDepartmentName_ReturnsBadRequest()
        {
            var ctrl = BuildController(new DepartmentsController(_mockDb.Object));
            var result = await ctrl.Create(new CreateDepartmentRequest { UnitID = 1, DepartmentName = "" });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        // ── WeeksController ────────────────────────────────────────────────────

        [Fact]
        public async void Weeks_Create_NullBody_ReturnsBadRequest()
        {
            var ctrl = BuildController(new WeeksController(_mockDb.Object));
            var result = await ctrl.Create(null);
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Weeks_Create_ZeroWeekNumber_ReturnsBadRequest()
        {
            var ctrl = BuildController(new WeeksController(_mockDb.Object));
            var result = await ctrl.Create(new CreateWeekRequest { WeekNumber = 0, Year = 2026 });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void Weeks_Create_ZeroYear_ReturnsBadRequest()
        {
            var ctrl = BuildController(new WeeksController(_mockDb.Object));
            var result = await ctrl.Create(new CreateWeekRequest { WeekNumber = 15, Year = 0 });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        // ── ActionTypesController ──────────────────────────────────────────────

        [Fact]
        public async void ActionTypes_Create_NullBody_ReturnsBadRequest()
        {
            var ctrl = BuildController(new ActionTypesController(_mockDb.Object));
            var result = await ctrl.Create(null);
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void ActionTypes_Create_EmptyTypeName_ReturnsBadRequest()
        {
            var ctrl = BuildController(new ActionTypesController(_mockDb.Object));
            var result = await ctrl.Create(new CreateActionTypeRequest { TypeName = "" });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        // ── ActionStatusesController ───────────────────────────────────────────

        [Fact]
        public async void ActionStatuses_Create_NullBody_ReturnsBadRequest()
        {
            var ctrl = BuildController(new ActionStatusesController(_mockDb.Object));
            var result = await ctrl.Create(null);
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void ActionStatuses_Create_MissingStatusKey_ReturnsBadRequest()
        {
            var ctrl = BuildController(new ActionStatusesController(_mockDb.Object));
            var result = await ctrl.Create(new CreateActionStatusRequest
            {
                StatusKey = "",
                StatusLabel = "In Progress",
                ColorHex = "#10b981",
                BgColorHex = "#ecfdf5"
            });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        [Fact]
        public async void ActionStatuses_Create_MissingColorHex_ReturnsBadRequest()
        {
            var ctrl = BuildController(new ActionStatusesController(_mockDb.Object));
            var result = await ctrl.Create(new CreateActionStatusRequest
            {
                StatusKey = "progress",
                StatusLabel = "In Progress",
                ColorHex = "",
                BgColorHex = "#ecfdf5"
            });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }

        // ── ActionStatusHistoryController ──────────────────────────────────────

        [Fact]
        public async void ActionStatusHistory_AddHistory_ZeroChangedBy_ReturnsBadRequest()
        {
            var ctrl = BuildController(new ActionStatusHistoryController(_mockDb.Object));
            var result = await ctrl.AddHistory(1, new CreateStatusHistoryRequest { ChangedBy = 0, StatusID = 1 });
            Assert.IsType<BadRequestErrorMessageResult>(result);
        }
    }
}
