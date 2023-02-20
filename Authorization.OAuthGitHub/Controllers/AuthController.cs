using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.OAuthGitHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            // Return back to the swagger after auth loop is completed.
            var authProperties = new AuthenticationProperties { RedirectUri = "https://localhost:7292/swagger/index.html" };

            return Challenge(authProperties, "GitHub");
        }

        [HttpGet("user-info")]
        public IActionResult UserInfo()
        {
            var principal = User.Identity as ClaimsIdentity;

            if (principal is null || !principal.IsAuthenticated)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                Claims = principal.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList()
            });
        }
    }
}