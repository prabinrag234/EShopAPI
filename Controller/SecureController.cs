using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopAPI.Controller
{
    [ApiController]
    [Route("secure")]
    public class SecureController : ControllerBase
    {
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var name = User.Identity?.Name ?? "unknown";

            return Ok(new
            {
                user = name,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }

}
