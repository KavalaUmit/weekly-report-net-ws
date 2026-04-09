using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace WeeklyReportWS.Data
{
    public static class DbConnectionFactory
    {
        private static readonly string _connectionString;

        static DbConnectionFactory()
        {
            // Priority: environment variable → App.config → exception
            _connectionString =
                Environment.GetEnvironmentVariable("WEEKLY_REPORT_CONNECTION_STRING")
                ?? ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
                ?? throw new InvalidOperationException(
                    "No connection string found. Set the WEEKLY_REPORT_CONNECTION_STRING " +
                    "environment variable or configure 'DefaultConnection' in App.config.");
        }

        public static SqlConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
