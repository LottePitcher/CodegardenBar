using Tweetinvi.Models;

namespace CgBarBackend.Services
{
    public interface ITwitterWebhookHandler
    {
        void Initialize(IAccountActivityRequestHandler accountActivityRequestHandler);
        bool AddUser(long userId);
        void AddMissingSubscriptions(IWebhookEnvironmentSubscriptions existingUserSubscriptions);
    }
}