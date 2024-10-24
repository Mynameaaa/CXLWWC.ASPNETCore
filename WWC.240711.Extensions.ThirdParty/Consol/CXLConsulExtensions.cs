using Consul;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Infrastructure;
using WWC._240711.Extensions.ThirdParty.Models;

namespace WWC._240711.Extensions.ThirdParty.Consol
{
    public static class CXLConsulExtensions
    {

        public static IServiceCollection AddCXLConsul(this IServiceCollection services)
        {
            var consulOptions = Appsettings.app<List<ConsulClientOptions>?>("ConsulClientOptions");

            if (consulOptions != null && consulOptions.Any())
            {
                // 注册 Consul 服务
                services.AddSingleton<IConsulClient, ConsulClient>(p =>
                {
                    var consulService = consulOptions.First();
                    var consulAddress = consulService.Host + ":" + consulService.Port;
                    return new ConsulClient(config => config.Address = new Uri(consulAddress));
                });
            }

            return services.AddSingleton<IConsulRegisterService>(ConsulRegisterService.CreateInstance());
        }


    }
}
