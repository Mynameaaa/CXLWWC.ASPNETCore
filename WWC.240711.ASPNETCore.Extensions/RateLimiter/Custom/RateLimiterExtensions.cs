using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.RateLimiting;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;
using WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options;
using WWC._240711.ASPNETCore.Infrastructure;
using ConcurrencyLimiterOptions = WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options.ConcurrencyLimiterOptions;
using FixedWindowRateLimiterOptions = WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options.FixedWindowRateLimiterOptions;
using SlidingWindowRateLimiterOptions = WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options.SlidingWindowRateLimiterOptions;
using TokenBucketRateLimiterOptions = WWC._240711.ASPNETCore.Extensions.RateLimiter.Custom.Options.TokenBucketRateLimiterOptions;

namespace WWC._240711.ASPNETCore.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddRateLimiterSetup(this IServiceCollection serivces)
        {
            if (!Appsettings.app<bool>("UseRateLimiter"))
                return serivces;

            Console.WriteLine("【限流策略已注册】");

            if (Appsettings.app<bool>("UseResponseRateLimiterOptions"))
            {
                var rateOptions = Appsettings.app<RateLimiterOptions>("ResponseRateLimiterOptions");
                serivces.Configure<RateLimiterOptions>(options => options = rateOptions);
            }

            serivces.AddRateLimiter(_ =>
            {
                if (Appsettings.app<bool>("UseFixedWindowLimiter"))
                {
                    var fixedWindowLimiterOptions = Appsettings.app<FixedWindowRateLimiterOptions>("FixedWindowRateLimiterOptions");
                    _
                    .AddFixedWindowLimiter(policyName: "fixed", fixedOptions =>
                    {
                        fixedOptions.PermitLimit = 4;
                        fixedOptions.Window = TimeSpan.FromSeconds(60);
                        fixedOptions.QueueLimit = 2;
                        fixedOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        fixedOptions.AutoReplenishment = true;
                    });
                }

                if (Appsettings.app<bool>("UseSlidingWindowLimiter"))
                {
                    var slidingWindowLimiterOptions = Appsettings.app<SlidingWindowRateLimiterOptions>("SlidingWindowRateLimiterOptions");
                    _
                    .AddSlidingWindowLimiter(policyName: "sliding", slidingOptions =>
                    {
                        slidingOptions.PermitLimit = 100;
                        slidingOptions.Window = TimeSpan.FromSeconds(30);
                        slidingOptions.QueueLimit = 2;
                        slidingOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        slidingOptions.AutoReplenishment = true;
                        slidingOptions.SegmentsPerWindow = 3;
                    });
                }

                if (Appsettings.app<bool>("UseTokenBucketLimiter"))
                {
                    var slidingWindowLimiterOptions = Appsettings.app<TokenBucketRateLimiterOptions>("TokenBucketRateLimiterOptions");
                    _
                    .AddTokenBucketLimiter(policyName: "token_bucket", tokenBucketOptions =>
                    {
                        tokenBucketOptions.TokenLimit = 4;
                        tokenBucketOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                        tokenBucketOptions.TokensPerPeriod = 2;
                        tokenBucketOptions.QueueLimit = 2;
                        tokenBucketOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        tokenBucketOptions.AutoReplenishment = true;
                    });
                }

                if (Appsettings.app<bool>("UseConcurrencyLimiter"))
                {
                    var slidingWindowLimiterOptions = Appsettings.app<ConcurrencyLimiterOptions>("ConcurrencyLimiterOptions");
                    _
                    .AddConcurrencyLimiter(policyName: "concurrency", concurrencyOptions =>
                    {
                        concurrencyOptions.PermitLimit = 4;
                        concurrencyOptions.QueueLimit = 2;
                        concurrencyOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    });
                }

                var globalOptions = Appsettings.app<GlobalRateLimiterOptions>("GlobalRateLimiterOptions");
                if (globalOptions?.UseGlobalRateLimiter ?? false)
                {
                    switch (globalOptions.GlobalRateLimiterType)
                    {
                        case GlobalRateLimiterType.FixedWindow:
                            _.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                            {
                                var fixedOptions = Appsettings.app<FixedWindowRateLimiterOptions>("FixedWindowRateLimiterOptions");

                                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                                // 针对非回环地址限流
                                if (!IPAddress.IsLoopback(remoteIpAddress!))
                                {
                                    return RateLimitPartition.GetFixedWindowLimiter
                                    (remoteIpAddress!, _ =>
                                        new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                                        {
                                            QueueProcessingOrder = fixedOptions?.QueueProcessingOrder ?? QueueProcessingOrder.OldestFirst,
                                            QueueLimit = fixedOptions?.QueueLimit ?? 5,
                                            PermitLimit = fixedOptions?.PermitLimit ?? 100,
                                            AutoReplenishment = fixedOptions?.AutoReplenishment ?? true,
                                            Window = TimeSpan.FromSeconds(fixedOptions?.Window ?? 10)
                                        });
                                }

                                // 若为回环地址，则不限流
                                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                            });
                            break;
                        case GlobalRateLimiterType.SlidingWindow:
                            _.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                            {
                                var slidingOptions = Appsettings.app<SlidingWindowRateLimiterOptions>("SlidingWindowRateLimiterOptions");

                                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                                // 针对非回环地址限流
                                if (!IPAddress.IsLoopback(remoteIpAddress!))
                                {
                                    return RateLimitPartition.GetSlidingWindowLimiter
                                    (remoteIpAddress!, _ =>
                                        new System.Threading.RateLimiting.SlidingWindowRateLimiterOptions
                                        {
                                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                            QueueLimit = slidingOptions?.QueueLimit ?? 5,
                                            PermitLimit = slidingOptions?.PermitLimit ?? 100,
                                            Window = TimeSpan.FromSeconds(slidingOptions?.Window ?? 10),
                                            AutoReplenishment = slidingOptions?.AutoReplenishment ?? true,
                                            SegmentsPerWindow = slidingOptions?.SegmentsPerWindow ?? 5,
                                        });
                                }

                                // 若为回环地址，则不限流
                                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                            });
                            break;
                        case GlobalRateLimiterType.TokenBucket:
                            _.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                            {
                                var tokenBucketRateLimiterOptions = Appsettings.app<TokenBucketRateLimiterOptions>("TokenBucketRateLimiterOptions");

                                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                                // 针对非回环地址限流
                                if (!IPAddress.IsLoopback(remoteIpAddress!))
                                {
                                    return RateLimitPartition.GetTokenBucketLimiter
                                    (remoteIpAddress!, _ =>
                                        new System.Threading.RateLimiting.TokenBucketRateLimiterOptions
                                        {
                                            TokenLimit = tokenBucketRateLimiterOptions?.TokenLimit ?? 4,
                                            QueueProcessingOrder = tokenBucketRateLimiterOptions?.QueueProcessingOrder ?? QueueProcessingOrder.OldestFirst,
                                            QueueLimit = 2,
                                            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                                            TokensPerPeriod = 10,
                                            AutoReplenishment = true,
                                        });
                                }

                                // 若为回环地址，则不限流
                                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                            });
                            break;
                        case GlobalRateLimiterType.Concurrency:
                            _.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                            {
                                var concurrencyLimiterOptions = Appsettings.app<ConcurrencyLimiterOptions>("ConcurrencyLimiterOptions");

                                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                                // 针对非回环地址限流
                                if (!IPAddress.IsLoopback(remoteIpAddress!))
                                {
                                    return RateLimitPartition.GetConcurrencyLimiter
                                    (remoteIpAddress!, _ =>
                                        new System.Threading.RateLimiting.ConcurrencyLimiterOptions
                                        {
                                            QueueProcessingOrder = concurrencyLimiterOptions?.QueueProcessingOrder ?? QueueProcessingOrder.OldestFirst,
                                            QueueLimit = concurrencyLimiterOptions?.QueueLimit ?? 5,
                                            PermitLimit = concurrencyLimiterOptions?.PermitLimit ?? 100
                                        });
                                }

                                // 若为回环地址，则不限流
                                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                            });
                            break;
                        default:
                            _.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                            {
                                var tokenBucketRateLimiterOptions = Appsettings.app<TokenBucketRateLimiterOptions>("TokenBucketRateLimiterOptions");

                                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                                // 针对非回环地址限流
                                if (!IPAddress.IsLoopback(remoteIpAddress!))
                                {
                                    return RateLimitPartition.GetTokenBucketLimiter
                                    (remoteIpAddress!, _ =>
                                        new System.Threading.RateLimiting.TokenBucketRateLimiterOptions
                                        {
                                            TokenLimit = tokenBucketRateLimiterOptions?.TokenLimit ?? 4,
                                            QueueProcessingOrder = tokenBucketRateLimiterOptions?.QueueProcessingOrder ?? QueueProcessingOrder.OldestFirst,
                                            QueueLimit = 2,
                                            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                                            TokensPerPeriod = 10,
                                            AutoReplenishment = true,
                                        });
                                }

                                // 若为回环地址，则不限流
                                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                            });
                            break;
                    }

                }

            });

            return serivces;
        }

        public static WebApplication UseRateLimiterSetup(this WebApplication app)
        {
            if (!Appsettings.app<bool>("UseRateLimiter"))
                return app;
            app.UseRateLimiter();
            return app;
        }
    }
}
