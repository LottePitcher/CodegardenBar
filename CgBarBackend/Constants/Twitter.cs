using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CgBarBackend.Constants
{
    public class Twitter
    {
        public const string BaseWebhookUrl = "Webhooks/Twitter";

        public static class Events
        {
            public const string TweetCreate = "tweet_create_events";
            public const string TweetDelete = "tweet_delete_events";
            public const string Favorite = "favorite_events"; 
            public const string Follow = "follow_events"; 
            public const string Unfollow = "unfollow_events";
            public const string Block = "block_events";
            public const string Unblock = "unblock_events";
            public const string Mute = "mute_events";
            public const string Unmute = "unmute_events";
            public const string User = "user_event";

            public static class DirectMessage
            {
                public const string Message = "direct_message_events";
                public const string IndicateTyping = "direct_message_indicate_typing_events";
                public const string MarkRead = "direct_message_mark_read_events";
                
            }
            
        }
    }
}
