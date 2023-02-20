using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer.Configs
{
    public static class InMemoryConfiguration
    {
        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static List<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("WeatherForecastApi.Read")
            };
        }

        public static List<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },
                    AllowedScopes = { "WeatherForecastApi.Read" },
                },
                new Client
                {
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    ClientId = "client_id_code",
                    ClientSecrets = { new Secret("client_secret_code".ToSha256()) },
                    AllowedScopes = { "WeatherForecastApi.Read" }
                }
            };
        }
    }
}
