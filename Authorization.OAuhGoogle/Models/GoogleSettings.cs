namespace Authorization.OAuhGoogle.Models
{
    public class GoogleSettings
    {
        public const string SectionName = "GoogleSettings";

        public string ClientId { get; set; }

        public string Secret { get; set; }
    }
}
