namespace Shared.Configuration.RateLimit
{
    public class RateLimitBase
    {
        public int PermitLimit { get; set; }

        public int WindowSeconds { get; set; }

        public int QueueLimit { get; set; }
    }
}
