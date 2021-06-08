using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Authorization;
using CgBarBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CgBarBackend.Controllers
{
    [RequireConfigPasswordFilter("BarTender:AdminPassword")]
    [Route("BarAdmin/[action]")]
    public class BarAdminController : ControllerBase
    {
        private readonly IBarTender _barTender;

        public BarAdminController(IBarTender barTender)
        {
            _barTender = barTender;
        }

        public bool Ping()
        {
            return true;
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
    }
}
