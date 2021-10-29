using System.Threading.Tasks;
using CgBarBackend.Discord.Attributes;
using Discord.Commands;

namespace ApexDiscord.Bot.Commands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        [HelpIgnore]
        [Command("ping")]
        [Summary("Replies pong")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }

        [HelpIgnore]
        [Command("ping")]
        [Summary("Replies pong")]
        public async Task PingWithData(
            [Summary("Also sends back the data you send")]string data
        )
        {
            await ReplyAsync($"pong {data}");
        }

        // ReplyAsync is a method on ModuleBase 
    }
}
