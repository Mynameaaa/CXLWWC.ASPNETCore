using Consul;
using Microsoft.AspNetCore.Builder;
using Ocelot.Values;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Infrastructure;
using WWC._240711.Extensions.ThirdParty.Models;

namespace WWC._240711.Extensions.ThirdParty;

public class ConsulRegisterService : IConsulRegisterService
{
    private ConsulRegisterService()
    {

    }

    private static IConsulRegisterService consulRegister = new ConsulRegisterService();
    public List<string> _disposedServices = new List<string>();
    private static List<ConsulClientOptions>? consulOptions = Appsettings.app<List<ConsulClientOptions>?>("ConsulClientOptions");

    /// <summary>
    /// 获取构造函数
    /// </summary>
    /// <returns></returns>
    public static IConsulRegisterService CreateInstance()
    {
        return consulRegister;
    }

    /// <summary>
    /// 注册 Consul 服务
    /// </summary>
    /// <param name="client"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> RegisterService(ConsulRegisterServiceModel model)
    {
        if (!HasConsulPoint())
            return false;

        // 健康检查配置
        var healthCheck = new AgentCheckRegistration
        {
            HTTP = Path.Combine(model.ServiceHost, model.HealthRoute), // 健康检查 URL
            Interval = TimeSpan.FromSeconds(10),
            Timeout = TimeSpan.FromSeconds(5),
        };

        // 服务注册信息
        var registration = new AgentServiceRegistration
        {
            ID = model.ServiceID,
            Name = model.ServiceName,
            Address = model.ServiceHost, // 或使用动态 IP
            Port = model.Port, // 服务端口
            Tags = model.Tags, // 标签
            Check = healthCheck,
        };

        var registerResult = await ResgisterService(registration);

        if (model.IsDisposed && registerResult)
        {
            _disposedServices.Add(model.ServiceID);
        }

        return registerResult;
    }

    /// <summary>
    /// 注销服务
    /// </summary>
    /// <param name="client"></param>
    /// <param name="serviceID"></param>
    /// <returns></returns>
    public async Task<bool> StopServiceAsync(string serviceID)
    {
        if (!HasConsulPoint())
            return false;

        var res = await StopServicesAsync([serviceID]);
        _disposedServices.Remove(serviceID);
        return res;
    }

    /// <summary>
    /// 清空服务列表
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public async Task<bool> StopServicesAsync()
    {
        if (!HasConsulPoint())
            return false;

        if (_disposedServices.Any())
        {
            var res = await StopServicesAsync(_disposedServices);
            _disposedServices.Clear();
            return res;
        }
        return true;
    }

    /// <summary>
    /// 是否存在 Consul 节点
    /// </summary>
    /// <returns></returns>
    public bool HasConsulPoint()
    {
        return consulOptions != null && consulOptions.Any();
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <param name="registration"></param>
    /// <returns></returns>
    private async Task<bool> ResgisterService(AgentServiceRegistration registration)
    {
        if (!HasConsulPoint())
            return false;

        // 遍历所有的 Consul 地址，分别注册服务
        foreach (var model in consulOptions!)
        {
            var address = model.Host + ":" + model.Port;
            try
            {
                using (var consulClient = new ConsulClient(config => config.Address = new Uri(address)))
                {
                    await consulClient.Agent.ServiceRegister(registration);
                    Log.Logger.Information($"服务注册成功，Service：【{registration.Address + ":" + registration.Port}】，ServiceID：【{registration.ID}】，Consul：【{address}】");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"连接到 Consul 失败，Consul 连接信息：【{address}】");
            }
        }

        return false;
    }

    /// <summary>
    /// 注销服务
    /// </summary>
    /// <param name="serviceID"></param>
    /// <returns></returns>
    private async Task<bool> StopServicesAsync(List<string> serviceIDs)
    {
        if (!HasConsulPoint())
            return false;

        // 遍历所有的 Consul 地址，分别注册服务
        foreach (var model in consulOptions!)
        {
            var address = model.Host + ":" + model.Port;
            try
            {
                using (var consulClient = new ConsulClient(config => config.Address = new Uri(address)))
                {
                    foreach (var serviceID in serviceIDs)
                    {
                        await consulClient.Agent.ServiceDeregister(serviceID);
                        Log.Logger.Information($"服务注销成功，ServiceID：【{serviceID}】");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"连接到 Consul 失败，Consul 连接信息：【{address}】");
            }
        }

        return false;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {

    }

}
