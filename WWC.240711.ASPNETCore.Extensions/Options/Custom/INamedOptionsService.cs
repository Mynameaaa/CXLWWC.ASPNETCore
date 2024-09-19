using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Http.Custom;

namespace WWC._240711.ASPNETCore.Extensions
{
    public interface INamedOptionsService
    {

        /// <summary>
        /// 获取 HttpClient 配置
        /// </summary>
        /// <returns></returns>
        Task<List<NamedHttpClientOptions>?> GetHttpClientOptions();

    }
}
