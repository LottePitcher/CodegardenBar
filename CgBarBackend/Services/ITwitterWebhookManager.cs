using Tweetinvi.Models;

namespace CgBarBackend.Services
{
    public interface ITwitterWebhookManager
    {
        void Initialize(IAccountActivityRequestHandler accountActivityRequestHandler);
        bool AddUser(long userId);
        void AddMissingSubscriptions(IWebhookEnvironmentSubscriptions existingUserSubscriptions);
    }
}