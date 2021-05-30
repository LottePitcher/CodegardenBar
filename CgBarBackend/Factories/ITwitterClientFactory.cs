using Tweetinvi;

namespace CgBarBackend.Factories
{
    public interface ITwitterClientFactory
    {
        TwitterClient BearerTokenClient { get; }
        TwitterClient ConsumerOnlyTwitterClient { get; }
        TwitterClient AccessTokenTwitterClient { get; }
        TwitterClient ConsumerBearerTwitterClient { get; }
    }
}