using Microsoft.AspNetCore.Mvc;

namespace CgBarBackend.Controllers
{
    [Route("Status/[action]")]
    public class StatusController : ControllerBase
    {
        public bool Ping()
        {
            return true;
        }
    }

}
