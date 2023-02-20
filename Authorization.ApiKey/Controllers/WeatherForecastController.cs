using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.ApiKey.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            var userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            userIdentity.AddClaims(new[]
            {
                new Claim("name", "User"),
                new Claim("permission", "Read.All")
            }); ;

            var userPrincipal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(userPrincipal);

            return NoContent();
        }

        [Authorize]
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var systemIdentity = User.Identities.FirstOrDefault(x => x.AuthenticationType == "System via api key");

            if (systemIdentity is not null)
            {
                _logger.LogInformation("Request is executed using api key.");
            }

            // Besides api key authentication there is could be other authentications, for example cookie.
            var userIdentity = User.Identities.FirstOrDefault(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme);

            if (userIdentity is not null)
            {
                _logger.LogInformation("Request is executed by user.");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}