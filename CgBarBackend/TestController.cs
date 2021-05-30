using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Factories;
using CgBarBackend.Services;
using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;

namespace CgBarBackend
{
    [Route("Test/[action]")]
    public class TestController : ControllerBase
    {
        private readonly ITwitterClientFactory _twitterClientFactory;

        public TestController(ITwitterClientFactory twitterClientFactory)
        {
            _twitterClientFactory = twitterClientFactory;
        }

        public bool Test()
        {
            return true;
        }

        public async Task<object> GetUserName(string userName)
        {
            var client = _twitterClientFactory.AccessTokenTwitterClient;
            var user = await client.Users.GetUserAsync(userName).ConfigureAwait(false);
            return new {user.Name, user.Location, user.Description};
        }

        public async Task<object> TweetCountdown(string send = null)
        {
            var openingTimeUtc = new DateTime(2021, 06, 09, 18, 0, 0, DateTimeKind.Utc);
            var timeUntilOpening = openingTimeUtc - DateTime.UtcNow;

            var days = DisplayNumberWithUnit(timeUntilOpening.TotalDays, "day");
            var hours = DisplayNumberWithUnit(timeUntilOpening.Hours, "hour");
            var minutes = DisplayNumberWithUnit(timeUntilOpening.Minutes, "minute");

            var displayTimeUntil = $"{days} {hours} and {minutes}";
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
                var client = new TwitterClient(_twitterCredentialsSupplier.GetTwitterCredentials());
                var tweet = await client.Tweets.PublishTweetAsync(messages[offset]);
                return new { message = message, tweet_sent = true, tweetId = tweet.Id};
            }

            return new { message = message, tweet_sent = false };
        }

        public async Task<IWebhookEnvironment[]> WebHookTests()
        {
            //var client = new TwitterClient(new TwitterCredentials
            //    {BearerToken = _twitterCredentialsSupplier.ApplicationBearerToken});
            var client = _twitterClientFactory.BearerTokenClient;
            var environments = await client.AccountActivity.GetAccountActivityWebhookEnvironmentsAsync().ConfigureAwait(false);

            return environments;

            //var twitterCtx = new TwitterContext(new ApplicationOnlyAuthorizer());

            //var searchResponse =
            //    await
            //        (from search in twitterCtx.Search
            //            where search.Type == SearchType.Search &&
            //                  search.Query == "\"LINQ to Twitter\""
            //            select search)
            //        .SingleOrDefaultAsync();
        }

        private string DisplayNumberWithUnit(double number, string singular, string plural = null)
        {
            if (plural == null) plural = singular + "s";
            var displayNumber = Convert.ToInt32(number);
            return (displayNumber == 1) ? displayNumber + " " + singular : displayNumber + " " + plural;
        }
    }

}
