using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Store token in cookies.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOAuth("GitHub", configure =>
    {
        // From this guide.
        // https://docs.github.com/en/developers/apps/building-oauth-apps/authorizing-oauth-apps

        configure.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        configure.ClientId = "<ClientId>";
        configure.ClientSecret = "<ClientSecret>";

        configure.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        configure.TokenEndpoint = "https://github.com/login/oauth/access_token";
        configure.UserInformationEndpoint = "https://api.github.com/user";
        configure.CallbackPath = "/oauth/github-callback";

        // Save tokens in 'AuthProperties'
        configure.SaveTokens = true;

        configure.Events.OnCreatingTicket = async context =>
        {
            // Set user claims in token.
            using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            using var result = await context.Backchannel.SendAsync(request);

            var userData = await result.Content.ReadFromJsonAsync<JsonElement>();
            context.RunClaimActions(userData);
        };

        // Configure mapping between `userData` that we get from github and what we set in token.
        configure.ClaimActions.MapJsonKey("custom_claim_id", "id");
        configure.ClaimActions.MapJsonKey("custom_claim_name", "name");
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
