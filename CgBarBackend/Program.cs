using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CgBarBackend.Discord;

namespace CgBarBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // we are splitting up the hostbuilder so we can register the discord bot
            // dependencies in the same container and run it before the site runs (as the site is blocking
            var hostBuilder = CreateHostBuilder(args);
            hostBuilder.ConfigureServices(DiscordBot.ConfigureDependencies);
            var buildHostBuilder = hostBuilder.Build();
            DiscordBot.Startup(buildHostBuilder.Services);
            buildHostBuilder.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json.local", optional: false, reloadOnChange: true);
                });

        
    }
}
