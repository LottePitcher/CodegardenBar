using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CgBarBackend.Hubs
{
    public interface ITwitterBarHub
    {
        Task TweetFavorited();
        Task Ping();
    }

    public class TwitterBarHub : Hub<ITwitterBarHub>
    {
        
    }
}
