using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CgBarBackend.Hubs
{
    public static class TwitterBarHubExtensions
    {
        public static async Task Ping(this IHubContext<TwitterBarHub, ITwitterBarHub> hub)
        {
            await hub.Clients.All.Ping().ConfigureAwait(false);
        }

        public static async Task NotifyAllTweetFavorited(this IHubContext<TwitterBarHub, ITwitterBarHub> hub)
        {
            await hub.Clients.All.TweetFavorited().ConfigureAwait(false);
        }
    }
}
