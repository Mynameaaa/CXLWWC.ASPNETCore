using System.Threading.RateLimiting;

namespace WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options
{
    public class ConcurrencyLimiterOptions
    {
        public int PermitLimit { get; set; }

        public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;

        public int QueueLimit { get; set; }
    }
}
