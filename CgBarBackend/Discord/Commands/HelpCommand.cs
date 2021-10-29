using System;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Discord.Attributes;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace CgBarBackend.Discord.Commands
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;

        public HelpCommand(CommandService service, IConfiguration config, IServiceProvider serviceProvider)
        {
            _service = service;
            _config = config;
            _serviceProvider = serviceProvider;
        }

        [HelpIgnore]
        [Command("help")]
        public async Task Help()
        {
            string prefix = _config["DiscordBotPrefix"];

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Available commands"
            };

            foreach (var command in _service.Modules.SelectMany(m => m.Commands))
            {
                // check if it should be ignored
                if (command.Attributes.Any(p => p.GetType() == typeof(HelpIgnoreAttribute)))
                {
                    continue;
                }

                // check preconditions
                var meetsPreconditions = true;
                foreach (var preCon in command.Preconditions)
                {
                    var result = await preCon.CheckPermissionsAsync(Context, command, _serviceProvider);
                    if (result.IsSuccess == false)
                    {
                        meetsPreconditions = false;
                        break;
                    }
                }
                if (meetsPreconditions == false)
                {
                    continue;
                }

                // render the command
                builder.AddField(x =>
                {
                    var aliases = command.Aliases.Where(a => a.ToLower() != command.Name.ToLower());

                    x.Name = $"{prefix}{command.Name} {string.Join(" ", command.Parameters)}";
                    x.Value = command.Summary != null && command.Summary.Trim().Length > 0 ? command.Summary : "No summary available.";
                    if (aliases.Any())
                    {
                        x.Value = "Alias(es): " + string.Join(',', aliases.Select(a => prefix + a)) + "\n" +x.Value;
                    }
                    x.IsInline = false;
                });
            }

            await Context.User.SendMessageAsync("", false, builder.Build());
        }

        [HelpIgnore]
        [Command("help")]
        public async Task HelpAsync(string command)
        {
            var retval = _service.Search(Context, command);

            if (!retval.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            string prefix = _config["DiscordBotPrefix"];
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in retval.Commands)
            {
                var cmd = match.Command;

                // check preconditions
                var meetsPreconditions = true;
                foreach (var preCon in cmd.Preconditions)
                {
                    var result = await preCon.CheckPermissionsAsync(Context, cmd, _serviceProvider);
                    if (result.IsSuccess == false)
                    {
                        meetsPreconditions = false;
                        break;
                    }
                }
                if (meetsPreconditions == false)
                {
                    continue;
                }

                builder.AddField(x =>
                {
                    x.Name = $"{prefix}{cmd.Name} {string.Join(" ", cmd.Parameters)}";
                    x.Value = $"Summary: {cmd.Summary} \n" +
                              string.Join("\n", cmd.Parameters.Select(p => $"{p.Name}: {p.Summary}"));
                              
                    x.IsInline = false;
                });
            }

            await Context.User.SendMessageAsync("", false, builder.Build());
        }

        private async Task<bool> CheckCommandPermissions(PreconditionAttribute precon, 
            ICommandContext context, 
            CommandInfo command,
            IServiceProvider serviceProvider)
        {
            return (await precon.CheckPermissionsAsync(Context, command, _serviceProvider)).IsSuccess;
        }
    }
}
