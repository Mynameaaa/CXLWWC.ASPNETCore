using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;
using WWC._240711.ASPNETCore.Extensions.Http.Custom;

namespace WWC._240711.ASPNETCore.Extensions.Options.Custom
{
    public static class CXLOptionsExtensions
    {
        /// <summary>
        /// 添加获取 Options 服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCXLHttpClientOptions(this IServiceCollection services)
        {
            return services.AddTransient<INamedOptionsService, NamedOptionsService>();
        }

        /// <summary>
        /// 配置 NamedHttpClientOptions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="sectionKey"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureCXLNamedHttpClient(this IServiceCollection services, string sectionKey = null)
        {
            return services.ConfigureOptions<NamedHttpClientConfigureOptions>();
        }

    }
}
