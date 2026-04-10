using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using WeeklyReportWS.Data;
using WeeklyReportWS.Controllers;

namespace WeeklyReportWS.Infrastructure
{
    public class SimpleDependencyResolver : IDependencyResolver
    {
        private readonly IDbConnectionFactory _db;

        public SimpleDependencyResolver(IDbConnectionFactory db)
        {
            _db = db;
        }

        public IDependencyScope BeginScope() => this;

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ActionsController))           return new ActionsController(_db);
            if (serviceType == typeof(ActionItemsController))       return new ActionItemsController(_db);
            if (serviceType == typeof(ActionStatusesController))    return new ActionStatusesController(_db);
            if (serviceType == typeof(ActionStatusHistoryController)) return new ActionStatusHistoryController(_db);
            if (serviceType == typeof(ActionTypesController))       return new ActionTypesController(_db);
            if (serviceType == typeof(DepartmentsController))       return new DepartmentsController(_db);
            if (serviceType == typeof(LinesController))             return new LinesController(_db);
            if (serviceType == typeof(UnitsController))             return new UnitsController(_db);
            if (serviceType == typeof(UsersController))             return new UsersController(_db);
            if (serviceType == typeof(WeeksController))             return new WeeksController(_db);
            if (serviceType == typeof(HealthController))            return new HealthController();
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType) => Array.Empty<object>();

        public void Dispose() { }
    }
}
