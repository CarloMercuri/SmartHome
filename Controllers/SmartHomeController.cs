using Microsoft.AspNetCore.Mvc;
using SmartHome.Controllers.Security;
using SmartHome.Controllers.Security.Attributes;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SmartHomeController : SecureAccessController
    {
        [Route("LightsControl")]
        [AuthenticationRequired]
        [HttpPost]

        public IActionResult LightsControl([FromBody] LightControlRequest _request)
        {
            try
            {
                Console.WriteLine(  );
                return Ok();
            }
            catch (Exception ex)
            {
                return base.StatusCode((int)HttpStatusCode.InternalServerError, "");
            }
        }
    }

    public class LightControlRequest
    {
        public string LightId { get; set; }
        public bool TurnOn { get; set; }
    }
}
