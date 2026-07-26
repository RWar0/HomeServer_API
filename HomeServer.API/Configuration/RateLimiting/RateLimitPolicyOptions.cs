namespace HomeServer.API.Configuration.RateLimiting
{
    public sealed class RateLimitPolicyOptions
    {
        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
        public int SegmentsPerWindow { get; set; }

        public RateLimitKeyType KeyType { get; set; }
    }
}
