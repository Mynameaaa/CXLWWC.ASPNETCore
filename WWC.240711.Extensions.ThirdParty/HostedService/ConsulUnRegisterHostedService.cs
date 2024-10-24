using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using WWC._240711.Extensions.ThirdParty.Consol;

namespace WWC._240711.ASPNETCore.Extensions;

public class ConsulUnRegisterHostApplicationLifetime : IHostedService
{
	private readonly IConsulRegisterService _consulRegisterService;
	private readonly IHostApplicationLifetime _hostApplicationLifetime;

	public ConsulUnRegisterHostApplicationLifetime(IConsulRegisterService consulRegisterService,
												   IHostApplicationLifetime hostApplicationLifetime)
	{
		_consulRegisterService = consulRegisterService;
		_hostApplicationLifetime = hostApplicationLifetime;

		// 注册 ApplicationStarted 事件
		_hostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
		// 注册 ApplicationStopping 事件
		_hostApplicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
	}

	// 处理应用启动时的逻辑
	private void OnApplicationStarted()
	{
		Log.Information("应用启动成功，Consul 注册服务");
	}

	// 处理应用停止时的逻辑
	private void OnApplicationStopping()
	{
		Log.Information("应用停止中，准备注销 Consul 服务");
        _consulRegisterService.StopServicesAsync().Wait();
	}

	// 这些是 IHostedService 接口的必要实现，可以用来处理服务的启动和停止。
	public Task StartAsync(CancellationToken cancellationToken)
	{
		// 应用启动时的任务
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		// 应用关闭时的任务
		return Task.CompletedTask;
	}
}
