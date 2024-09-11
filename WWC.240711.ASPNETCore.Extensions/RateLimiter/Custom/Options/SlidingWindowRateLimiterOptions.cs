using System.Threading.RateLimiting;

namespace WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options
{
    public sealed class SlidingWindowRateLimiterOptions
    {
        public int Window { get; set; }

        public int SegmentsPerWindow { get; set; }

        public bool AutoReplenishment { get; set; } = true;

        public int PermitLimit { get; set; }

        public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;

        public int QueueLimit { get; set; }
    }
}
