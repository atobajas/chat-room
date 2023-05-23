using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
