namespace Authorization.OAuhGoogle.Models
{
    public class AuthRequest
    {
        public IDictionary<string, string?> ToQueryParams()
        {
            return new Dictionary<string, string?>
            {
                ["client_id"] = ClientId,
                ["redirect_uri"] = RedirectUri,
                ["response_type"] = ResponseType,
                ["scope"] = Scope,
                ["access_type"] = AccessType,
                ["include_granted_scopes"] = IncludeGrantedScopes.ToString(),
            };
        }

        public string ClientId { get; set; }

        public string RedirectUri { get; set; }

        public string ResponseType { get; } = "code";

        public string Scope { get; set; }

        public string IncludeGrantedScopes { get; } = "true";

        public string AccessType { get; } = "offline";
    }
}
