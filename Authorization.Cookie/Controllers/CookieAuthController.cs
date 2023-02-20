using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Cookie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CookieAuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(string permission = "view")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, Guid.NewGuid().ToString()),
                new Claim("permission", permission)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IssuedUtc = DateTime.UtcNow,
                IsPersistent = false,

                // This `ExpiresUtc` override `ExpireTimeSpan` property configured in Program.cs.
                ExpiresUtc = DateTime.UtcNow.AddSeconds(60),
            };

            // Set `.AspNetCore.Cookies` access cookie with specified claims.
            // This cookie is HttpOnly, Secure and has SameSite=Lax.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            return NoContent();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            // Removes the access cookie.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // In general you can achieve the same behaviour by doing this:
            // HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");

            return NoContent();
        }

        [HttpGet("claims")]
        [Authorize]
        public IActionResult Claims()
        {
            var user1 = User;
            var user2 = HttpContext.User;

            var areEqual = user1 == user2; // it is true

            return Ok(new
            {
                Claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value })
            });
        }

        [HttpGet("view-protected")]
        [Authorize(Policy = "View")]
        public IActionResult Protected()
        {
            return Ok(new { Message = "Success" });
        }

        [HttpPut("edit-protected")]
        [Authorize(Policy = "Edit")]
        public IActionResult EditProtected()
        {
            return Ok(new { Message = "Success" });
        }
    }
}
