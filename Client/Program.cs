using IdentityModel.Client;

using var client = new HttpClient();

// Get access token using Client credentials flow.
var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:1001");
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = discoveryDocument.TokenEndpoint,
    ClientId = "client_id",
    ClientSecret = "client_secret",
    Scope = "WeatherForecastApi.Read"
});

// Set token in `Authorization` header.
client.SetBearerToken(tokenResponse.AccessToken);

// Request data from resource.
var weatherResponse = await client.GetAsync("https://localhost:7149/api/WeatherForecast/GetWeatherForecast");
weatherResponse.EnsureSuccessStatusCode();
var content = await weatherResponse.Content.ReadAsStringAsync();
