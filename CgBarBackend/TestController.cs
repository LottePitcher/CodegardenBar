using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;

namespace CgBarBackend
{
    [Route("Test/[action]")]
    public class TestController : ControllerBase
    {
        private readonly ITwitterCredentialsSupplier _twitterCredentialsSupplier;
        private readonly TwitterClient _client;

        public TestController(ITwitterCredentialsSupplier twitterCredentialsSupplier, TwitterClient client)
        {
            _twitterCredentialsSupplier = twitterCredentialsSupplier;
            _client = client;
        }

        public bool Test()
        {
            return true;
        }

        public async Task<object> GetUserName(string userName)
        {
            var client = new TwitterClient(_twitterCredentialsSupplier.GetTwitterCredentials());
            var user = await client.Users.GetUserAsync(userName).ConfigureAwait(false);
            return new {user.Name, user.Location, user.Description};
        }

        public async Task<IWebhookEnvironment[]> WebHookTests()
        {
            var environments = await _client.AccountActivity.GetAccountActivityWebhookEnvironmentsAsync().ConfigureAwait(false);

            return environments;
        }
    }

}
