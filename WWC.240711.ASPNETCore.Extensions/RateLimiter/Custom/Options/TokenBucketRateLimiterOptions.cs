using System.Threading.RateLimiting;

namespace WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options
{
    public sealed class TokenBucketRateLimiterOptions
    {
        public int ReplenishmentPeriod { get; set; }

        public int TokensPerPeriod { get; set; }

        public bool AutoReplenishment { get; set; } = true;

        public int TokenLimit { get; set; }

        public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;

        public int QueueLimit { get; set; }
    }
}
