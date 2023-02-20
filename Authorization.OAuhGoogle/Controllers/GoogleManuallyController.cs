using System.Net.Http.Headers;
using Authorization.OAuhGoogle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Authorization.OAuhGoogle.Controllers
{
    // Performing auth flow manually. 
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleManuallyController : ControllerBase
    {
        private const string GoogleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string GoogleTokenUrl = "https://oauth2.googleapis.com/token";
        private const string RedirectUrl = "https://localhost:7192/api/GoogleAuth/Code";
        private static readonly HttpClient _httpClient = new();

        private readonly IOptions<GoogleSettings> _options;

        public GoogleManuallyController(IOptions<GoogleSettings> options)
        {
            _options = options;
        }

        // 1) Redirect user to Google login page specifying required query params in uri.
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var authRequest = new AuthRequest
            {
                ClientId = _options.Value.ClientId,
                Scope = "https://www.googleapis.com/auth/youtube",
                RedirectUri = RedirectUrl,
            };

            var url = QueryHelpers.AddQueryString(GoogleAuthUrl, authRequest.ToQueryParams());

            return Redirect(url);
        }

        // 2) Exchange code for the access token (expires very soon) and refresh token (has long expiration period).
        [HttpGet("code")]
        public async Task<IActionResult> Code()
        {
            var tokenRequest = new TokenRequest
            {
                ClientId = _options.Value.ClientId,
                Secret = _options.Value.Secret,
                Code = HttpContext.Request.Query["code"],
                RedirectUrl = RedirectUrl
            };

            var content = new FormUrlEncodedContent(tokenRequest.ToQueryParams());
            var result = await _httpClient.PostAsync(GoogleTokenUrl, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(resultContent);

            return Ok(tokenResponse);
        }

        // No need to prompt user authorize again once access token is expired.
        // We got long live refresh token and we can use it to get new pair of tokens.
        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var refreshRequest = new RefreshTokenRequest
            {
                ClientId = _options.Value.ClientId,
                Secret = _options.Value.Secret,
                RefreshToken = refreshToken,
            };

            var content = new FormUrlEncodedContent(refreshRequest.ToQueryParams());
            var result = await _httpClient.PostAsync(GoogleTokenUrl, content);
            var resultContent = await result.Content.ReadAsStringAsync();

            // New access and refresh token.
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(resultContent);

            return Ok(tokenResponse);
        }

        // Test endpoint to verify that access token works.
        [HttpGet("my-youtube-subscribtions")]
        public async Task<IActionResult> MyYouTubeSubscriptions(string accessToken)
        {
            var mySubsRequest = new GetMySubscriptionsRequest();
            var url = QueryHelpers.AddQueryString(
                "https://youtube.googleapis.com/youtube/v3/subscriptions",
                mySubsRequest.ToQueryParams());

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var result = await client.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();

            return Ok(resultContent);
        }
    }
}
