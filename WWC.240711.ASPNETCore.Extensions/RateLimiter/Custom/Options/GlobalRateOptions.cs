using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options
{
    public class GlobalRateLimiterOptions
    {
        public const string FixedWindow = "FixedWindowRateLimiterOptions";
        public const string SlidingWindow = "SlidingWindowRateLimiterOptions";
        public const string TokenBucket = "TokenBucketRateLimiterOptions";
        public const string Concurrency = "ConcurrencyLimiterOptions";

        public bool UseGlobalRateLimiter { get; set; }

        public GlobalRateLimiterType GlobalRateLimiterType { get; set; }   

    }
    public enum GlobalRateLimiterType
    {
        FixedWindow = 1,
        SlidingWindow = 2,
        TokenBucket = 3,
        Concurrency = 4,
    }
}
