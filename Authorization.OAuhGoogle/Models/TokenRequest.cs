namespace Authorization.OAuhGoogle.Models
{
    public class TokenRequest
    {
        public IDictionary<string, string> ToQueryParams()
        {
            return new Dictionary<string, string>
            {
                ["client_id"] = ClientId,
                ["client_secret"] = Secret,
                ["code"] = Code,
                ["grant_type"] = GrantType,
                ["redirect_uri"] = RedirectUrl,
            };
        }

        public string ClientId { get; set; }

        public string Secret { get; set; }

        public string Code { get; set; }

        public string GrantType { get; } = "authorization_code";

        public string RedirectUrl { get; set; }
    }
}
