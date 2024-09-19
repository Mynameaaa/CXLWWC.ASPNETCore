using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            services.AddSingleton<IValidateOptions<List<NamedHttpClientOptions>>, NamedHttpClientOptionsValidator>();
            return services.ConfigureOptions<NamedHttpClientConfigureOptions>();
        }

    }
}
