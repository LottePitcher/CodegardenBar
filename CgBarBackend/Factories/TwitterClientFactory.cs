using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Services;
using Tweetinvi;
using Tweetinvi.Models;

namespace CgBarBackend.Factories
{
    public class TwitterClientFactory : ITwitterClientFactory
    {
        private readonly ITwitterCredentialsSupplier _twitterCredentialsSupplier;

        public TwitterClientFactory(ITwitterCredentialsSupplier twitterCredentialsSupplier)
        {
            _twitterCredentialsSupplier = twitterCredentialsSupplier;
        }

        public TwitterClient ApplicationBearerTokenOnlyClient => new TwitterClient(new TwitterCredentials()
            {BearerToken = _twitterCredentialsSupplier.ApplicationBearerToken});

        public TwitterClient ApplicationClient => new TwitterClient(
            new TwitterCredentials(_twitterCredentialsSupplier.ConsumerKey,
                _twitterCredentialsSupplier.ConsumerKeySecret));

        public TwitterClient UserClient => new TwitterClient(
            new TwitterCredentials(_twitterCredentialsSupplier.ConsumerKey,
                _twitterCredentialsSupplier.ConsumerKeySecret, _twitterCredentialsSupplier.AccessToken, _twitterCredentialsSupplier.AccessTokenSecret));

        public TwitterClient ApplicationBearerTokenClient => new TwitterClient(
            new TwitterCredentials(_twitterCredentialsSupplier.ConsumerKey,
                _twitterCredentialsSupplier.ConsumerKeySecret, _twitterCredentialsSupplier.ApplicationBearerToken));
    }
}
