using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebhookTester.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var configurationBuilder = new ConfigurationBuilder();
                    configurationBuilder.AddEnvironmentVariables();
                    config.AddConfiguration(configurationBuilder.Build());
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                        logging.AddDebug();

                    logging.AddConsole();
                    logging.AddEventSourceLogger();
                })
                .UseStartup<Startup>();

    }
}
