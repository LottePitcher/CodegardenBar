using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Factories;
using CgBarBackend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.WebLogic;

namespace CgBarBackend.Services
{
    public class TwitterWebhookManager : ITwitterWebhookManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TwitterWebhookManager> _logger;
        private readonly IBarTender _barTender;
        private readonly ITwitterClientFactory _twitterClientFactory;
        private IAccountActivityRequestHandler _accountActivityRequestHandler;

        private long _userId = 0;

        private ConcurrentBag<long> _handledUsers = new ConcurrentBag<long>();

        public TwitterWebhookManager(IConfiguration configuration,
            ILogger<TwitterWebhookManager> logger,
            IBarTender barTender,
            ITwitterClientFactory twitterClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _barTender = barTender;
            _twitterClientFactory = twitterClientFactory;
            long.TryParse(_configuration["TwitterApi:UserId"], out _userId);
        }

        public void Initialize(IAccountActivityRequestHandler accountActivityRequestHandler)
        {
            _accountActivityRequestHandler = accountActivityRequestHandler;
        }

        // todo change return attempt
        public bool AddUser(long userId)
        {
            _logger.LogInformation("Adding User {userId}", userId);
            if (_accountActivityRequestHandler == null)
            {
                _logger.LogWarning("accountActivityRequestHandler is null");
                return false;
            }

            LinkUserSubscriptions(userId);

            _handledUsers.Add(userId);
            return true;
        }

        public void AddMissingSubscriptions(IWebhookEnvironmentSubscriptions existingUserSubscriptions)
        {
            foreach (var webhookSubscription in existingUserSubscriptions.Subscriptions)
            {
                if (long.TryParse(webhookSubscription.UserId, out var userId) && _handledUsers.Contains(userId) == false)
                {
                    AddUser(userId);
                }
            }
        }

        private void LinkUserSubscriptions(long userId)
        {
            var accountActivitySteam = _accountActivityRequestHandler.GetAccountActivityStream(userId, _configuration["TwitterApi:Environment"]);

            accountActivitySteam.TweetCreated += async (sender, tweetCreatedEvent) =>
            {
                await ProcessTweetIntent(tweetCreatedEvent).ConfigureAwait(false);
                _logger.LogInformation("Tweet created: {0}", tweetCreatedEvent.Json);
            };
        }

        private async Task ProcessTweetIntent(TweetCreatedEvent tweetCreatedEvent)
        {
            var client = _twitterClientFactory.UserClient;

            _logger.LogInformation("ProcessTweetIntent() with {@tweetCreatedEvent}", tweetCreatedEvent);

            // add the calling patron if not in the bar yet
            if (_barTender.PatronExists(tweetCreatedEvent.Tweet.CreatedBy.ScreenName) == false)
            {
                _logger.LogInformation("Patron does not exists, adding {screenName}",
                    tweetCreatedEvent.Tweet.CreatedBy.ScreenName);
                _barTender.AddPatron(tweetCreatedEvent.Tweet.CreatedBy.ScreenName, tweetCreatedEvent.Tweet.CreatedBy.Name, tweetCreatedEvent.Tweet.CreatedBy.ProfileImageUrl400x400);
            }
            // get all mentions that are not the caller and not the app
            var otherPatronMentions = tweetCreatedEvent.Tweet.UserMentions.Where(um =>
                um.Id != _userId && um.ScreenName != tweetCreatedEvent.Tweet.CreatedBy.ScreenName).ToList();

            // get all mentions that are not yet in the bar
            var newPatronsMentions = otherPatronMentions.Where(um => _barTender.PatronExists(um.ScreenName) == false);
            foreach (var userMention in newPatronsMentions) //todo put the id in config
            {
                var user = await client.Users.GetUserAsync(userMention.ScreenName).ConfigureAwait(false);
                _barTender.AddPatron(user.ScreenName, user.Name, user.ProfileImageUrl400x400, tweetCreatedEvent.Tweet.CreatedBy.ScreenName);
            }

            // find the drink and polite words
            var drinks = _barTender.Drinks;
            var tweetWords = tweetCreatedEvent.Tweet.Text.Split(new char[] { ' ', '\n', '\r' });
            var foundDrink = drinks.FirstOrDefault(allowedDrink => tweetWords.Any(w => string.Equals(allowedDrink, w, StringComparison.InvariantCultureIgnoreCase)));
            var foundPoliteWord = _barTender.PoliteWords.FirstOrDefault(politeWord => tweetWords.Any(w => string.Equals(politeWord, w, StringComparison.InvariantCultureIgnoreCase)));

            // if no drink is found, ignore the call
            if (foundDrink == null || foundDrink.Trim().Length <= 0)
            {
                _logger.LogInformation("No drink {@drink} found in {@drinks}", tweetWords, drinks);
                return;
            }

            // if its an order for other people, give them all a drink but add the politeness to the caller
            if (otherPatronMentions.Any())
            {
                foreach (var patron in otherPatronMentions)
                {
                    _logger.LogInformation("OrderDrink to: {toPatron}, {drink}, {fromPatron} ", patron.ScreenName, foundDrink, tweetCreatedEvent.Tweet.CreatedBy.ScreenName); 
                    _barTender.OrderDrink(patron.ScreenName, foundDrink, byScreenName: tweetCreatedEvent.Tweet.CreatedBy.ScreenName, polite: foundPoliteWord?.Any() == true);
                }
            }
            // else create an order for the caller
            else
            {
                _logger.LogInformation("OrderDrink (single) to: {toPatron}, {drink}, {fromPatron} ", tweetCreatedEvent.Tweet.CreatedBy.ScreenName, foundDrink, tweetCreatedEvent.Tweet.CreatedBy.ScreenName); 

                _barTender.OrderDrink(tweetCreatedEvent.Tweet.CreatedBy.ScreenName, foundDrink, polite: foundPoliteWord?.Any() == true);
            }
        }
    }
}
