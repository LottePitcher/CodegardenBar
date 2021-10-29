using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CgBarBackend.Discord
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private ILogger<CommandHandler>? _logger;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commands)
        {
            _services = services;
            _commands = commands;
            _config = services.GetService<IConfiguration>();
            _logger = services.GetService<ILogger<CommandHandler>>();
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                services: _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if ((message.HasStringPrefix(_config["DiscordBotPrefix"], ref argPos) ||
                 message.HasMentionPrefix(_client.CurrentUser, ref argPos)) == false
                || message.Author.IsBot)
            {
                return;
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);

            if (!result.IsSuccess)
            {
                _logger.LogInformation("Could not execute command: " + result.ErrorReason);
            }
        }
    }
}
