using Tweetinvi;

namespace CgBarBackend.Factories
{
    public interface ITwitterClientFactory
    {
        TwitterClient ApplicationBearerTokenOnlyClient { get; }
        TwitterClient ApplicationClient { get; }
        TwitterClient UserClient { get; }
        TwitterClient ApplicationBearerTokenClient { get; }
    }
}