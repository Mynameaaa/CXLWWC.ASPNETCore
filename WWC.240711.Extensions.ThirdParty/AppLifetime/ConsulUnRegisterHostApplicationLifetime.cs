using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.Extensions.ThirdParty.Consol;

namespace WWC._240711.Extensions.ThirdParty.AppLifetime
{
    public class ConsulUnRegisterHostApplicationLifetime : IHostApplicationLifetime
    {
        private readonly IConsulRegisterService _consulRegisterService;

        public ConsulUnRegisterHostApplicationLifetime(IConsulRegisterService consulRegisterService)
        {
            _consulRegisterService = consulRegisterService;
            ApplicationStarted.Register(() =>
            {
                Log.Logger.Information("Consul 服务释放 HostLifetime 成功开启");
            });
        }

        private CancellationTokenSource ApplicationStartedSource = new CancellationTokenSource();
        private CancellationTokenSource ApplicationStoppedSource = new CancellationTokenSource();
        private CancellationTokenSource ApplicationStoppingSource = new CancellationTokenSource();

        /// <summary>
        /// 启动
        /// </summary>
        public CancellationToken ApplicationStarted => ApplicationStartedSource.Token;

        /// <summary>
        /// 关闭
        /// </summary>
        public CancellationToken ApplicationStopped => ApplicationStoppedSource.Token;

        /// <summary>
        /// 关闭中
        /// </summary>
        public CancellationToken ApplicationStopping => ApplicationStoppingSource.Token;

        /// <summary>
        /// 关闭应用程序
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void StopApplication()
        {
            var result = _consulRegisterService.StopServicesAsync().Result;
        }
    }
}
