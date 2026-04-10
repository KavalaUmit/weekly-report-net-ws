using System;
using Microsoft.Owin.Hosting;

namespace WeeklyReportWS.Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string baseUrl = "http://localhost:4443";

            using (WebApp.Start<WeeklyReportWS.Startup>(baseUrl))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("===========================================");
                Console.WriteLine($"  Weekly Report API running at {baseUrl}");
                Console.WriteLine($"  Swagger UI: {baseUrl}/swagger");
                Console.WriteLine("  Press ENTER to stop...");
                Console.WriteLine("===========================================");
                Console.ResetColor();
                Console.ReadLine();
            }
        }
    }
}
