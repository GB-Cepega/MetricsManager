using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new ConfigurationBuilder()
             .SetBasePath(System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();

            NLog.LogManager.Configuration = new NLogLoggingConfiguration(logger.GetSection("NLog"));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5050");
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // �������� ����������� �����������
                    logging.SetMinimumLevel(LogLevel.Trace); // ������������� ����������� ������� �����������
                })
                .UseNLog(); // ��������� ���������� nlog
    }
}
