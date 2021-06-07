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

        Task PatronAdded(Patron patron);
        Task DrinkOrdered(Patron patron);
        Task DrinkExpired(string patronScreenName);
        Task PatronExpired(string patronScreenName);

    }

    public class TwitterBarHub : Hub<ITwitterBarHub>
    {
    }
}
