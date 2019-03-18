using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SC.DevChallenge.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
					Console.WriteLine($"Hosting environment is {env.EnvironmentName}");
                    config.AddJsonFile("appsettings.json", optional: true,
                            reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                            optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .UseUrls(new ConfigurationBuilder().AddCommandLine(args).Build()["urls"])
                .UseSerilog((context, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration))
                .UseStartup<Startup>();
    }
}
