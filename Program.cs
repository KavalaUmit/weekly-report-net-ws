using System;
using System.Configuration;
using Microsoft.Owin.Hosting;

namespace WeeklyReportWS
{
    class Program
    {
        static void Main(string[] args)
        {
            string port = ConfigurationManager.AppSettings["Port"] ?? "4443";
            string baseAddress = $"http://0.0.0.0:{port}/";

            Console.WriteLine($"Starting weekly-report-ws on {baseAddress}");
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Server started. Press Enter to stop...");
                Console.ReadLine();
            }
        }
    }
}
