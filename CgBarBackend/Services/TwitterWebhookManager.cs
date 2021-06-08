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
                _logger.LogInformation("Tweet created: {0}",tweetCreatedEvent.Json);
            };
        }

        private async Task ProcessTweetIntent(TweetCreatedEvent tweetCreatedEvent)
        {
            var client = _twitterClientFactory.UserClient;
            
            _barTender.AddPatron(tweetCreatedEvent.Tweet.CreatedBy.ScreenName, tweetCreatedEvent.Tweet.CreatedBy.Name, tweetCreatedEvent.Tweet.CreatedBy.ProfileImageUrl400x400);
            foreach (var userMention in tweetCreatedEvent.Tweet.UserMentions.Where(um => um.Id != 1397223787469459468 && _barTender.PatronExists(um.ScreenName) == false)) //todo put the id in config
            {
                var user = await client.Users.GetUserAsync(userMention.ScreenName).ConfigureAwait(false);
                _barTender.AddPatron(user.ScreenName,user.Name,user.ProfileImageUrl400x400, tweetCreatedEvent.Tweet.CreatedBy.ScreenName);
            }

            //todo drinks

            //todo niceness
        }
    }
}
