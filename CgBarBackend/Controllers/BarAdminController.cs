using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CgBarBackend.Controllers
{
    [RequireConfigPasswordFilter("BarTender:AdminPassword")]
    [Route("BarAdmin/[action]")]
    public class BarAdminController : ControllerBase
    {
        public bool Ping()
        {
            return true;
        }
    }
}
