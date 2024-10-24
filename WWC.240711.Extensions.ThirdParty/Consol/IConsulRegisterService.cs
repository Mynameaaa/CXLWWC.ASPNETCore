using Consul;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.Extensions.ThirdParty.Consol
{
    public interface IConsulRegisterService : IDisposable
    {
        /// <summary>
        /// 注册 Consul 服务
        /// </summary>
        /// <param name="client"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> RegisterService(ConsulRegisterServiceModel model);

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serviceID"></param>
        /// <returns></returns>
        Task<bool> StopServiceAsync(string serviceID);

        /// <summary>
        /// 清空服务列表
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task<bool> StopServicesAsync();

        /// <summary>
        /// 是否存在 Consul 节点
        /// </summary>
        /// <returns></returns>
        public bool HasConsulPoint();

    }
}
