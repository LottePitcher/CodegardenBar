using Tweetinvi.Models;

namespace CgBarBackend.Services
{
    public interface ITwitterCredentialsSupplier
    {
        IReadOnlyTwitterCredentials GetTwitterCredentials();
        IReadOnlyConsumerCredentials GetConsumerOnlyCredentials();
        string EnvironmentName { get;}
    }
}