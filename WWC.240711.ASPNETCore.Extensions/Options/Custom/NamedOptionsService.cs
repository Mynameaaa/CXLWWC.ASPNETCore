using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;
using WWC._240711.ASPNETCore.Extensions.Http.Custom;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class NamedOptionsService : INamedOptionsService
    {
        private readonly IOptionsMonitor<List<NamedHttpClientOptions>?> _optionsMonitor;

        public NamedOptionsService(IOptionsMonitor<List<NamedHttpClientOptions>?> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        /// <summary>
        /// 获取 HttpClient 配置
        /// </summary>
        /// <returns></returns>
        public Task<List<NamedHttpClientOptions>?> GetHttpClientOptions()
        {
            return Task.FromResult(_optionsMonitor.CurrentValue);
        }
    }
}
