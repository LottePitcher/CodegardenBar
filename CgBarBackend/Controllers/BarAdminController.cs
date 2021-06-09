using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Authorization;
using CgBarBackend.Hubs;
using CgBarBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CgBarBackend.Controllers
{
    [RequireConfigPasswordFilter("BarTender:AdminPassword")]
    [Route("BarAdmin/[action]")]
    public class BarAdminController : ControllerBase
    {
        private readonly IBarTender _barTender;
        private readonly IHubContext<TwitterBarHub, ITwitterBarHub> _hubContext;

        public BarAdminController(IBarTender barTender, IHubContext<TwitterBarHub,ITwitterBarHub> hubContext)
        {
            _barTender = barTender;
            _hubContext = hubContext;
        }

        public bool Ping()
        {
            return true;
        }

        public async Task PingHubClients()
        {
            await _hubContext.Ping();
        }

        [HttpPost]
        public void AddPatron(string screenName, string name, string profileImage)
        {
            _barTender.AddPatron(screenName,name,profileImage);
        }

        [HttpPost]
        public void OrderDrink(string screenName, string drink)
        {
            _barTender.OrderDrink(screenName,drink);
        }

        [HttpPost]
        public void BanPatron(string screenName)
        {
            _barTender.BanPatron(screenName);
        }

        [HttpPost]
        public void UnBanPatron(string screenName)
        {
            _barTender.UnBanPatron(screenName);
        }

        [HttpPost]
        public void AddDrink(string name)
        {
            _barTender.AddDrink(name);
        }

        [HttpPost]
        public void RemoveDrink(string name)
        {
            _barTender.RemoveDrink(name);
        }

        public IEnumerable<string> Drinks()
        {
            return _barTender.Drinks;
        }

        [HttpPost] 
        public void AddPoliteWord(string name)
        {
            _barTender.AddPoliteWord(name);
        }

        [HttpPost]
        public void RemovePoliteWord(string name)
        {
            _barTender.RemovePoliteWord(name);
        }

        public IEnumerable<string> PoliteWords()
        {
            return _barTender.PoliteWords;
        }
    }
}
