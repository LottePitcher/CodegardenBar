using Tweetinvi.Models;

namespace CgBarBackend.Services
{
    public interface ITwitterCredentialsSupplier
    {
        IReadOnlyTwitterCredentials GetTwitterCredentials();
        IReadOnlyConsumerCredentials GetConsumerOnlyCredentials();
        string EnvironmentName { get;}
        string ConsumerKey { get; }
        string AccessToken { get; }
        string ConsumerKeySecret { get; }
        string AccessTokenSecret { get; }
        string ApplicationBearerToken { get; }
    }
}