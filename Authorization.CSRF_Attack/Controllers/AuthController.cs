using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.CSRF_Attack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var csrfKey = Guid.NewGuid().ToString();
            var token = new JwtSecurityToken(
                issuer: "issuer",
                audience: "audience",
                claims: new[] {
                    new Claim("user", "John"),
                    new Claim("csrf", csrfKey)
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key-key-key-key-key-key")), SecurityAlgorithms.HmacSha256));

            var jwtToken = tokenHandler.WriteToken(token);

            HttpContext.Response.Cookies.Append("access_token", jwtToken, new CookieOptions
            {
                HttpOnly = true,

                // SameSite option solves CSRF problem if it set to Lax or Strict.
                // SameSite = SameSiteMode.Lax
            });

            // Client should get `csrfKey` and use it in header for all requests.
            return Ok(new
            {
                CSRF = csrfKey,
            });
        }

        [Authorize]
        [HttpPost("form-submit")]
        // CSRF volnurable endpoint
        public IActionResult FormSubmit()
        {
            return Ok(new
            {
                Message = "Action done!"
            });
        }

        [Authorize]
        [CSRFValidation]
        [HttpPost("form-submit-v2")]
        // CSRF safe endpoint
        public IActionResult FormSubmitV2()
        {
            return Ok(new
            {
                Message = "Action v2 done!"
            });
        }
    }
}