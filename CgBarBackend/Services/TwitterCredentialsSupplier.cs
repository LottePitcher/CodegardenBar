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
        private readonly IConfiguration _configuration;

        private readonly ITwitterCredentials _credentials;
        private readonly IConsumerOnlyCredentials _consumerOnlyCredentials;

        public string ConsumerKey => _configuration?["TwitterApi:ConsumerKey"];
        public string AccessToken => _configuration?["TwitterApi:AccessToken"];
        public string ConsumerKeySecret => _configuration?["TwitterApi:ConsumerKeySecret"];
        public string AccessTokenSecret => _configuration?["TwitterApi:AccessTokenSecret"];
        public string ApplicationBearerToken => _configuration?["TwitterApi:ApplicationBearerToken"];
        public string EnvironmentName => _configuration?["TwitterApi:EnvironmentName"];

        public TwitterCredentialsSupplier(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new TwitterCredentials(
                configuration["TwitterApi:ConsumerKey"], configuration["TwitterApi:ConsumerKeySecret"],
                configuration["TwitterApi:AccessToken"], configuration["TwitterApi:AccessTokenSecret"]);
            _consumerOnlyCredentials = new ConsumerOnlyCredentials(configuration["TwitterApi:ConsumerKey"],
                configuration["TwitterApi:ConsumerKeySecret"])
            {
                BearerToken = configuration["TwitterApi:ApplicationBearerToken"]
            };
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
