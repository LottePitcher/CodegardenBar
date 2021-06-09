using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Models;
using Microsoft.AspNetCore.SignalR;

namespace CgBarBackend.Hubs
{
    public interface ITwitterBarHub
    {
        Task TweetFavorited();
        Task Ping();

        Task PatronAdded(PatronDto patron);
        Task DrinkOrdered(PatronDto patron);
        Task DrinkExpired(string patronScreenName);
        Task PatronExpired(string patronScreenName);
        Task PatronPolitenessChanged(PatronDto patron);

    }

    public class TwitterBarHub : Hub<ITwitterBarHub>
    {
    }
}
