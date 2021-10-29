using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CgBarBackend.Discord.Attributes;
using CgBarBackend.Services;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CgBarBackend.Discord.Commands
{
    public class OrderCommand : ModuleBase<SocketCommandContext>
    {
        private Regex _cleanRegex = new Regex("[^a-z0-9\n\r ]+", RegexOptions.IgnoreCase);
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        private ILogger<OrderCommand> _logger;

        public OrderCommand(CommandService service, IConfiguration config, IServiceProvider serviceProvider)
        {
            _service = service;
            _config = config;
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILogger<OrderCommand>>();
        }

        [Command("order")]
        public async Task Order([Remainder]string message)
        {
            _logger.LogInformation("Processing new order: " + message);
            var bartender = _serviceProvider.GetService<IBarTender>();

            // check if the current user is already a patron
            if (bartender.PatronExists(Context.User.Username) == false)
            {
                // if not create it
                bartender.AddPatron(Context.User.Username,Context.User.Mention, Context.User.GetAvatarUrl());
            }

            // figure out intent
            // todo transform this and TwiiterwebhookManager 113+ to IntentService
            // find the drink and polite words
            var drinks = bartender.Drinks;
            var cleanedTweetText = _cleanRegex.Replace(message, String.Empty);
            _logger.LogInformation("Cleaned tweet text: {@text}", cleanedTweetText);
            var tweetWords = cleanedTweetText.Split(new char[] { ' ', '\n', '\r' });
            var foundDrink = drinks.FirstOrDefault(allowedDrink => tweetWords.Any(w => string.Equals(allowedDrink, w, StringComparison.InvariantCultureIgnoreCase)));
            var foundPoliteWord = bartender.PoliteWords.FirstOrDefault(politeWord => tweetWords.Any(w => string.Equals(politeWord, w, StringComparison.InvariantCultureIgnoreCase)));
            var refillKeyword =
                tweetWords.Any(w => string.Equals("refill", w, StringComparison.InvariantCultureIgnoreCase));

            // if no drink is found, ignore the call
            if ((foundDrink == null || foundDrink.Trim().Length <= 0) && refillKeyword == false)
            {
                _logger.LogInformation("No drink {@drink} found in {@drinks}", tweetWords, drinks);
                return;
            }

            bartender.OrderDrink(Context.User.Username, foundDrink, polite: foundPoliteWord?.Any() == true);


            // order the drink for the patron
            Context.User.GetAvatarUrl();
        }
    }
}
