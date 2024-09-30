using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SmartHome.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class ConnectivityController : ControllerBase
    {
        [Route("TestConnectivity")]
        [HttpGet]

        public IActionResult TestConnectivity()
        {
            try
            {
                return Ok("Der er hul i gennem!");
            }
            catch (Exception ex)
            {
                return base.StatusCode((int)HttpStatusCode.InternalServerError, "");
            }
        }
    }
}
