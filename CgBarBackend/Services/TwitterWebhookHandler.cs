using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tweetinvi.Models;
using Tweetinvi.WebLogic;

namespace CgBarBackend.Services
{
    public class TwitterWebhookHandler : ITwitterWebhookHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TwitterWebhookHandler> _logger;
        private IAccountActivityRequestHandler _accountActivityRequestHandler;

        private ConcurrentBag<long> _handledUsers = new ConcurrentBag<long>();

        public TwitterWebhookHandler(IConfiguration configuration,
            ILogger<TwitterWebhookHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
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

            var accountActivitySteam = _accountActivityRequestHandler.GetAccountActivityStream(userId, _configuration["TwitterApi:Environment"]);

            accountActivitySteam.TweetFavorited += (sender, tweetCreatedEvent) =>
            {
                _logger.LogInformation("a tweet was liked");
            };

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
    }
}
