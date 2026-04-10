using System.Data;

namespace WeeklyReportWS.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
