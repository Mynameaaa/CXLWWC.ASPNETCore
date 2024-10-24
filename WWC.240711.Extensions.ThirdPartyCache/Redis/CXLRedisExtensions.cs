using CSRedis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Infrastructure;
using WWC._240711.Extensions.ThirdParty.Models;

namespace WWC._240711.Extensions.ThirdPartyCache.Redis
{
    public static class CXLRedisExtensions
    {

        /// <summary>
        /// 添加 Redis 作为缓存
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddCXLRedisCache(this IServiceCollection services)
        {
            var connectOptions = Appsettings.app<List<RedisConnectOptions>?>("RedisConnectOptions");
            if (connectOptions == null || !connectOptions.Any())
                return services;

            string[] redisClusterNodes = AssembleConnect(connectOptions);
            var csredis = new CSRedisClient(null, redisClusterNodes);
            RedisHelper.Initialization(csredis);
            return services.AddSingleton<CSRedisClient>(csredis);
        }

        /// <summary>
        /// 组装链接信息
        /// </summary>
        /// <param name="connectOptions"></param>
        /// <returns></returns>
        private static string[] AssembleConnect(List<RedisConnectOptions> connectOptions)
        {
            List<string> results = new List<string>();

            foreach (var connect in connectOptions)
            {
                StringBuilder connectSB = new StringBuilder();
                connectSB.Append(connect.Host);
                connectSB.Append(":");
                connectSB.Append(connect.Port);

                if (!string.IsNullOrEmpty(connect.Password))
                    connectSB.Append($";password={connect.Password}");

                if (connect.ConnectTimeout.HasValue)
                    connectSB.Append($";connectTimeout={connect.ConnectTimeout}");

                if (connect.ReadTimeout.HasValue)
                    connectSB.Append($";readtimeout={connect.ReadTimeout}");

                if (connect.WriteTimeout.HasValue)
                    connectSB.Append($";writetimeout={connect.WriteTimeout}");

                if (connect.PoolSize.HasValue)
                    connectSB.Append($";poolSize={connect.PoolSize}");

                if (connect.MaxIdleTime.HasValue)
                    connectSB.Append($";maxIdleTime={connect.MaxIdleTime}");

            }
            return results.ToArray();
        }

    }
}
