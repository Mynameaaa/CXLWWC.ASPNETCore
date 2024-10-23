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
        await _next.Invoke(context);

        // 记录当前请求信息
        using DownstreamResponse downstreamResponse = context.Items.DownstreamResponse();

        var statusCode = context.Response.StatusCode;

        if (downstreamResponse != null)
        {

        }

        // 获取转发的下游服务地址
        var downstreamUrl = context.Items["DownstreamRequest"]?.ToString();
        if (!string.IsNullOrEmpty(downstreamUrl))
        {
            // 记录下游服务地址
            Console.WriteLine($"下游请求地址: {downstreamUrl}");
            // 这里可以将日志记录到文件或其他地方
        }

    }

}
