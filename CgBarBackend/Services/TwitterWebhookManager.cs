using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            if (_accountActivityRequestHandler == null)
            {
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

            _barTender.AddPatron(tweetCreatedEvent.Tweet.CreatedBy.ScreenName, tweetCreatedEvent.Tweet.CreatedBy.Name, tweetCreatedEvent.Tweet.CreatedBy.ProfileImageUrl400x400);
            var mentionedPeople = tweetCreatedEvent.Tweet.UserMentions.Where(um =>
                um.Id != _userId && _barTender.PatronExists(um.ScreenName) == false).ToList();
            foreach (var userMention in mentionedPeople) //todo put the id in config
            {
                var user = await client.Users.GetUserAsync(userMention.ScreenName).ConfigureAwait(false);
                _barTender.AddPatron(user.ScreenName, user.Name, user.ProfileImageUrl400x400, tweetCreatedEvent.Tweet.CreatedBy.ScreenName);
            }

            var tweetWords = tweetCreatedEvent.Tweet.Text.Split(new char[] { ' ', '\n', '\r' });
            var foundDrink = _barTender.Drinks.FirstOrDefault(allowedDrink => tweetWords.Contains(allowedDrink));
            var foundPoliteWord = _barTender.PoliteWords.FirstOrDefault(politeWord => tweetWords.Contains(politeWord));

            if (foundDrink == null || foundDrink.Trim().Length <= 0)
            {
                return;

            }

            // todo: process politeness
            if (mentionedPeople.Any())
            {
                foreach (var person in mentionedPeople)
                {
                    _barTender.OrderDrink(person.ScreenName, foundDrink, byScreenName: tweetCreatedEvent.Tweet.CreatedBy.ScreenName);
                }
            }
            else
            {
                _barTender.OrderDrink(tweetCreatedEvent.Tweet.CreatedBy.ScreenName, foundDrink);
            }
        }
    }
}
