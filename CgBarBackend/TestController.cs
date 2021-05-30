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
    }

}
