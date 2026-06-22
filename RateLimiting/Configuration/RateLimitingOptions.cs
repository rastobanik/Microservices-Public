namespace Shared.Configuration.RateLimit
{
    public class RateLimitingOptions
    {
        public const string Section = "RateLimiting";

        public RateLimitBase Global { get; set; } = new RateLimitBase();

        public RateLimitBase Fixed { get; set; } = new RateLimitBase();
    }
}
