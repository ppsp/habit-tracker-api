using HabitTrackerTools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HabitTrackerWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.ConfigLogger();
            Logger.Debug("Starting HabitTracker");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.IsProduction())
                    {
                        var builtConfig = config.Build();
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
