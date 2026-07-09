namespace HomeServer.API.Configuration.RateLimiting
{
    public sealed class RateLimitingOptions
    {
        public const string SectionName = "RateLimiting";
        public RateLimitPolicyOptions Global { get; set; } = new();
        public Dictionary<string, RateLimitPolicyOptions> Policies { get; set; } = [];
    }
}
