﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Models;
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

        public static async Task NotifyAllPatronAdded(this IHubContext<TwitterBarHub, ITwitterBarHub> hub, Patron patron)
        {
            await hub.Clients.All.PatronAdded(patron).ConfigureAwait(false);
        }

        public static async Task NotifyAllDrinkOrdered(this IHubContext<TwitterBarHub, ITwitterBarHub> hub, Patron patron)
        {
            await hub.Clients.All.DrinkOrdered(patron).ConfigureAwait(false);
        }

        public static async Task NotifyAllDrinkExpired(this IHubContext<TwitterBarHub, ITwitterBarHub> hub, string patronScreenName)
        {
            await hub.Clients.All.DrinkExpired(patronScreenName).ConfigureAwait(false);
        }

        public static async Task NotifyAllPatronExpired(this IHubContext<TwitterBarHub, ITwitterBarHub> hub, string patronScreenName)
        {
            await hub.Clients.All.PatronExpired(patronScreenName).ConfigureAwait(false);
        }
    }
}
