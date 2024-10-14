using CSRedis;
using Microsoft.Extensions.DependencyInjection;

namespace WWC._240711.ASPNETCore.Auth;

public static class CXLCacheExtenions
{
    public static IServiceCollection AddCXLCSRedisCore(this IServiceCollection services)
    {
        var redis = new CSRedisClient("127.0.0.1:6379");
        RedisHelper.Initialization(redis);
        services.AddSingleton(redis);

        return services;
    }

    public static IServiceCollection AddCXLMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();

        return services;
    }

}
