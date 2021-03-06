using System;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Authorization;
using CgBarBackend.Factories;
using CgBarBackend.Hubs;
using CgBarBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Tweetinvi;
using Tweetinvi.Models;

namespace CgBarBackend.Controllers
{
    [RequireConfigPasswordFilter("TwitterApi:AdminPassword")]
    [Route("TwitterAdmin/[action]")]
    public class TwitterAdminController : ControllerBase
    {
        private readonly ITwitterClientFactory _twitterClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ITwitterWebhookManager _twitterWebhookManager;
        private readonly IHubContext<TwitterBarHub, ITwitterBarHub> _twitterBarHub;

        public TwitterAdminController(ITwitterClientFactory twitterClientFactory, IConfiguration configuration,
            IMemoryCache cache, ITwitterWebhookManager twitterWebhookManager,
            IHubContext<TwitterBarHub,ITwitterBarHub> twitterBarHub)
        {
            _twitterClientFactory = twitterClientFactory;
            _configuration = configuration;
            _cache = cache;
            _twitterWebhookManager = twitterWebhookManager;
            _twitterBarHub = twitterBarHub;
        }

        public bool Ping()
        {
            return true;
        }

        public async Task<object> TweetCountdown(string send = null)
        {
            var openingTimeUtc = new DateTime(2021, 06, 09, 18, 0, 0, DateTimeKind.Utc);
            var timeUntilOpening = openingTimeUtc - DateTime.UtcNow;

            var days = DisplayNumberWithUnit(timeUntilOpening.TotalDays, "day");
            var hours = DisplayNumberWithUnit(timeUntilOpening.Hours, "hour");
            var minutes = DisplayNumberWithUnit(timeUntilOpening.Minutes, "minute");

            var displayTimeUntil = $"{days}, {hours} and {minutes}";
            if (timeUntilOpening.TotalDays == 0)
            {
                displayTimeUntil = $"{hours} and {minutes}";
            }

            string[] messages = 
            {
                $"If all goes to plan ... we'll be opening in {displayTimeUntil}!",
                $"Not that we're worried or anything, but only {displayTimeUntil} until we open!",
                $"In {displayTimeUntil} we should be able to open the doors!",
                $"Someone call the brewery, there's only {displayTimeUntil} until we open!",
            };

            var rnd = new Random();
            var offset = rnd.Next(0, 3);
            var message = messages[offset];

            if (send == "y")
            {
                var client = _twitterClientFactory.UserClient;
                var tweet = await client.Tweets.PublishTweetAsync(messages[offset]);
                return new { message = message, tweet_sent = true, tweetId = tweet.Id};
            }

            return new { message = message, tweet_sent = false };
        }

        public async Task<IWebhookEnvironment[]> Environments()
        {
            var client = _twitterClientFactory.ApplicationBearerTokenClient;
            var environments = await client.AccountActivity.GetAccountActivityWebhookEnvironmentsAsync().ConfigureAwait(false);

            return environments;
        }

        public async Task<IWebhook> RegisterWebhook()
        {
            var appClient = _twitterClientFactory.ApplicationBearerTokenClient;
            var webhooks = await appClient.AccountActivity.GetAccountActivityEnvironmentWebhooksAsync(_configuration["TwitterApi:Environment"]);
            foreach (var webhook in webhooks)
            {
                await appClient.AccountActivity.DeleteAccountActivityWebhookAsync(_configuration["TwitterApi:Environment"],
                    webhook.Id);
            }

            var userClient = _twitterClientFactory.UserClient;
            var result = await userClient.AccountActivity.CreateAccountActivityWebhookAsync(_configuration["TwitterApi:Environment"],
                _configuration["TwitterApi:Host"] + Constants.Twitter.BaseWebhookUrl);
            return result;
        }

        public async Task<object> DeleteWebhook()
        {
            var appClient = _twitterClientFactory.ApplicationBearerTokenClient;
            var webhooks = await appClient.AccountActivity.GetAccountActivityEnvironmentWebhooksAsync(_configuration["TwitterApi:Environment"]);
            if (!webhooks.Any())
            {
                return new { webhooksCount = 0 };
            }

            var webhookId = webhooks.FirstOrDefault().Id;
            await appClient.AccountActivity.DeleteAccountActivityWebhookAsync(_configuration["TwitterApi:Environment"], webhookId);
            return new { deletedWebhookId = webhookId };
        }

        public async Task<object> StartSubscribeToAccount()
        {
            var appClient = _twitterClientFactory.ApplicationClient;
            var authRequest = await appClient.Auth.RequestAuthenticationUrlAsync();
            var authRequestKey = Guid.NewGuid();
            _cache.Set(authRequestKey, authRequest,
                new MemoryCacheEntryOptions {SlidingExpiration = new TimeSpan(0, 0, 10, 0)});
            

            return new { key = authRequestKey, url = authRequest.AuthorizationURL } ;
        }

        public async Task<bool> SubscribeToAccount(Guid authKey, string pin)
        {
            if (_cache.TryGetValue<IAuthenticationRequest>(authKey, out var cachedAuthRequest) == false)
            {
                return false;
            }

            var appClient = _twitterClientFactory.ApplicationBearerTokenClient;

            var userCredentials = await appClient.Auth.RequestCredentialsFromVerifierCodeAsync(pin, cachedAuthRequest);

            var userClient = new TwitterClient(userCredentials);

            await userClient.AccountActivity.SubscribeToAccountActivityAsync(_configuration["TwitterApi:Environment"]);

            var existingUserSubscriptions = await appClient.AccountActivity.GetAccountActivitySubscriptionsAsync(
                _configuration["TwitterApi:Environment"]);
            _twitterWebhookManager.AddMissingSubscriptions(existingUserSubscriptions);
            return true;
        }

        public async Task PingTwitterBarHub()
        {
            await _twitterBarHub.Clients.All.Ping();
        }

        public async Task<object> GetUserDetails(string userName)
        {
            var client = _twitterClientFactory.UserClient;
            var user = await client.Users.GetUserAsync(userName).ConfigureAwait(false);
            return new { user.Name, user.Location, user.Description, user.ProfileImageUrl400x400, user.ScreenName };
        }


        private string DisplayNumberWithUnit(double number, string singular, string plural = null)
        {
            if (plural == null) plural = singular + "s";
            var displayNumber = Convert.ToInt32(number);
            return (displayNumber == 1) ? displayNumber + " " + singular : displayNumber + " " + plural;
        }
    }

}
