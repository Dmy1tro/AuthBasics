using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Authorization.ApiKey.Middleware
{
    public class PreSharedKeyMiddleware
    {
        public const string PreSharedKeyHeaderName = "X-Api-Key";

        private readonly RequestDelegate _next;
        private readonly IOptions<ApiKeyConfig> _apiKeyConfig;

        public PreSharedKeyMiddleware(RequestDelegate next, IOptions<ApiKeyConfig> apiKeyConfig)
        {
            _next = next;
            _apiKeyConfig = apiKeyConfig;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(PreSharedKeyHeaderName, out var apiKey))
            {
                // If api key is wrong then send 401 response.
                if (apiKey != _apiKeyConfig.Value.ApiKey)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Api key mismatch.");
                    return;
                }

                // Create identity with necessary claims.
                var systemIdentity = new ClaimsIdentity("System via api key");
                systemIdentity.AddClaims(new[]
                {
                    new Claim("name", "System"),
                    new Claim("permission", "ReadWrite.All")
                });

                // Authorize attribute will allow request and request will be executed by 'System'.
                context.User.AddIdentity(systemIdentity);
            }

            await _next(context);
        }
    }
}
