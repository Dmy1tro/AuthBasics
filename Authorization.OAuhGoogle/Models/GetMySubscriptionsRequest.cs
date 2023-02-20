namespace Authorization.OAuhGoogle.Models
{
    public class GetMySubscriptionsRequest
    {
        public IDictionary<string, string?> ToQueryParams()
        {
            return new Dictionary<string, string?>
            {
                ["part"] = Part,
                ["mine"] = Mine.ToString(),
                ["maxResults"] = MaxResults.ToString(),
            };
        }

        public string Part { get; } = "snippet,contentDetails";

        public bool Mine { get; } = true;

        public int MaxResults { get; set; } = 1;
    }
}
