using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Tweetinvi.Models;

namespace CgBarBackend.Services
{
    public class TwitterCredentialsSupplier : ITwitterCredentialsSupplier
    {
        public string EnvironmentName { get; private set; }

        private readonly ITwitterCredentials _credentials;
        private readonly IConsumerOnlyCredentials _consumerOnlyCredentials;

        public TwitterCredentialsSupplier(IConfiguration configuration)
        {
            _credentials = new TwitterCredentials(
                configuration["TwitterApi:ConsumerKey"], configuration["TwitterApi:ConsumerKeySecret"],
                configuration["TwitterApi:AccessToken"], configuration["TwitterApi:AccessTokenSecret"]);
            _consumerOnlyCredentials = new ConsumerOnlyCredentials(configuration["TwitterApi:ConsumerKey"],
                configuration["TwitterApi:ConsumerKeySecret"])
            {
                BearerToken = configuration["TwitterApi:ApplicationBearerToken"]
            };
            EnvironmentName = configuration["TwitterApi:EnvironmentName"];
        }

        public IReadOnlyTwitterCredentials GetTwitterCredentials()
        {
            return _credentials;
        }

        public IReadOnlyConsumerCredentials GetConsumerOnlyCredentials()
        {
            return _consumerOnlyCredentials;
        }
    }
}
