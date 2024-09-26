using Microsoft.Extensions.Options;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class NamedOptionsService : INamedOptionsService
    {
        /// <summary>
        /// 获取 HttpClient 配置
        /// </summary>
        /// <returns></returns>
        public Task<List<NamedHttpClientOptions>?> GetHttpClientOptions()
        {
            return Task.FromResult(Appsettings.app<List<NamedHttpClientOptions>?>("NamedHttpClientOptions"));
        }
    }
}
