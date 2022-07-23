using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


namespace CgBarBackend.Discord
{
    public class DiscordBot
    {
        public static void ConfigureDependencies(IServiceCollection obj)
        {
            
        }

        public static async void Startup(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var logger = serviceProvider.GetService<ILogger<DiscordSocketClient>>();

            if (string.IsNullOrEmpty(config["DiscordBotToken"]))
            {
                logger.LogInformation("Discord bot token not set: Discord not enabled");
                return;
            }

            var client = new DiscordSocketClient();
            var commandService = new CommandService();
            var commandHandler = new CommandHandler(serviceProvider, client, commandService);
            await commandHandler.InstallCommandsAsync();

            client.Log += message => LogDiscordMessage(message, logger);

            await client.LoginAsync(TokenType.Bot, config["DiscordBotToken"]);
            var helpCommand = commandService.Commands.FirstOrDefault(c => c.Name == "help");
            if (helpCommand != null)
            {
                await client.SetActivityAsync(new Game(config["DiscordBotPrefix"] + helpCommand.Name, ActivityType.Watching));
            }
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private static async Task LogDiscordMessage(LogMessage message, ILogger<DiscordSocketClient> logger)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    logger.LogCritical(message.Exception, message.Message);
                    break;
                case LogSeverity.Error:
                    logger.LogError(message.Exception, message.Message);
                    break;
                case LogSeverity.Warning:
                    logger.LogWarning(message.Exception, message.Message);
                    break;
                case LogSeverity.Info:
                    logger.LogInformation(message.Exception, message.Message);
                    break;
                case LogSeverity.Verbose:
                    logger.LogTrace(message.Exception, message.Message);
                    break;
                case LogSeverity.Debug:
                    logger.LogDebug(message.Exception, message.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
