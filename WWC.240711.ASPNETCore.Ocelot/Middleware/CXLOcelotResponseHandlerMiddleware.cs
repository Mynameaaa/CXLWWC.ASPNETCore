using Microsoft.AspNetCore.Http;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Request.Middleware;

namespace WWC._240711.ASPNETCore.Ocelot.Middleware;

public class CXLOcelotResponseHandlerMiddleware : OcelotMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CXLOcelotResponseHandlerMiddleware> _logger;

    public CXLOcelotResponseHandlerMiddleware(IOcelotLoggerFactory loggerFactory
        , RequestDelegate next
        , ILogger<CXLOcelotResponseHandlerMiddleware> logger) : base(loggerFactory.CreateLogger<CXLOcelotResponseHandlerMiddleware>())
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 拦截上游服务器请求
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 记录当前请求信息
        var downstreamRequest = context.Items["DownstreamRequest"] as DownstreamRequest;

        await _next.Invoke(context);

        var statusCode = context.Response.StatusCode;

        //switch (statusCode)
        //{
        //    case 404:
        //        context.Response.Redirect("/not-found"); // 重定向到指定页面
        //        context.Response.StatusCode = StatusCodes.Status302Found; // 设置状态码为 302
        //        return;
        //    default:
        //        break;
        //}

        if (downstreamRequest != null)
        {
            var downstreamUrl = downstreamRequest.ToUri(); // 获取下游请求的完整 URL
            _logger.LogInformation($"下游地址：{downstreamUrl}");
        }


    }

}
