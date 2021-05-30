using CgBarBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tweetinvi.Models;

namespace CgBarBackend.Controllers
{
    [Route( Constants.Twitter.BaseWebhookUrl)]
    public class TwitterWebHookController : ControllerBase
    {
        private readonly ILogger<TwitterWebHookController> _logger;

        public TwitterWebHookController(ILogger<TwitterWebHookController>logger )
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("")]
        public void Index([FromBody]string body)
        {
            var type = TwitterWebhookFactory.EventType(body);
            if (type == null)
            {
                _logger.LogWarning("Received untreatable message",body);
            }

            switch (type)
            {
                case Constants.Twitter.Events.TweetCreate:
                    _logger.LogInformation("Handling Tweet(s) created event");
                    break;
                default:
                    _logger.LogDebug("Unhandled event", body);
                    break;
            }
        }

        [Route("Test")]
        public bool Test()
        {
            return true;
        }
    }

}
