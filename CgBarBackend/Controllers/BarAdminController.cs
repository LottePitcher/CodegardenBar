using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CgBarBackend.Controllers
{
    [RequireBarTenderAdminPasswordFilter]
    [Route("BarAdmin/[action]")]
    public class BarAdminController : ControllerBase
    {
        public bool Test()
        {
            return true;
        }
    }
}
