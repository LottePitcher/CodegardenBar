using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweetinvi.Core.DTO;
using TwitterEvents = CgBarBackend.Constants.Twitter.Events;

namespace CgBarBackend.Models
{
    public class TwitterWebhookFactory
    {
        public static readonly ReadOnlyCollection<string> EventNames = new ReadOnlyCollection<string>(
            new List<string>
            {
                TwitterEvents.TweetCreate, TwitterEvents.Favorite, TwitterEvents.Follow, TwitterEvents.Unfollow, TwitterEvents.Block,
                TwitterEvents.Unblock, TwitterEvents.Mute, TwitterEvents.Unmute, TwitterEvents.User, TwitterEvents.DirectMessage.Message,
                TwitterEvents.DirectMessage.IndicateTyping, TwitterEvents.DirectMessage.MarkRead, TwitterEvents.TweetDelete
            });

        public static string EventType(string jsonBody)
        {
            if (jsonBody == null)
            {
                return null;
            }


            var jsonObjectEvent = JObject.Parse(jsonBody);

            var jsonEventChildren = jsonObjectEvent.Children().ToArray();
            var keys = jsonEventChildren.Where(x => x.Path.EndsWith("event") || x.Path.EndsWith("events"));
            var key = keys.SingleOrDefault();

            if (key == null)
            {
                return null;
            }


            var eventName = key.Path;
            if (EventNames.Contains(eventName))
            {
                return eventName;
            }
            return null;
        }

        public static TweetCreatedEvents TweetCreatedEvent(string jsonBody)
        {
            return JsonConvert.DeserializeObject<TweetCreatedEvents>(jsonBody);
        }
    }

    public class TweetCreateEventsBase
    {
        [JsonProperty(propertyName: "for_user_id")]
        public long ForUserId { get; set; }
    }

    public class TweetCreatedEvents : TweetCreateEventsBase
    {
        [JsonProperty(propertyName: "user_has_blocked")]
        public long UserHasBlocked { get; set; }

        [JsonProperty(propertyName: "tweet_create_events")]
        public List<TweetDTO> Events { get; set; }
    }

    public class FavoriteEvents : TweetCreateEventsBase
    {
        [JsonProperty(propertyName: "favorite_events")]
        public List<FavoriteEvent> Events { get; set; }
    }

    public class FavoriteEvent
    {
        [JsonProperty(propertyName: "id")]
        public string Id { get; set; }

        [JsonProperty(propertyName: "created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty(propertyName: "timestamp_ms")]
        public long TimeStampMs { get; set; }

        [JsonProperty(propertyName: "favorited_status")]
        public TweetDTO FavoriteStatus { get; set; }

        [JsonProperty(propertyName: "user")]
        public UserDTO User { get; set; }
    }
}
