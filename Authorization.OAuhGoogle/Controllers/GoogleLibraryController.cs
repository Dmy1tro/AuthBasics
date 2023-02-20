using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.OAuhGoogle.Controllers
{
    // Performing auth flow using microsoft library.
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleLibraryController : ControllerBase
    {
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            // Return back to the swagger after auth loop is completed.
            var authProperties = new AuthenticationProperties { RedirectUri = "https://localhost:7192/swagger/index.html" };

            return Challenge("MicrosoftLibrary");
        }
    }
}
