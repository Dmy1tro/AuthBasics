namespace Authorization.OAuhGoogle.Models
{
    public class RefreshTokenRequest
    {
        public IDictionary<string, string> ToQueryParams()
        {
            return new Dictionary<string, string>
            {
                ["client_id"] = ClientId,
                ["client_secret"] = Secret,
                ["grant_type"] = GrantType,
                ["refresh_token"] = RefreshToken,
            };
        }

        public string ClientId { get; set; }

        public string Secret { get; set; }

        public string GrantType { get; } = "refresh_token";

        public string RefreshToken { get; set; }
    }
}
