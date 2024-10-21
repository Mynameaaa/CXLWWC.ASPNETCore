using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions;

public static class CXLSerilogExtensions
{

    /// <summary>
    /// 添加 Serilog 服务
    /// </summary>
    public static IHostBuilder AddCXLSerilog(this ConfigureHostBuilder hostBuilder)
    {
        string infoLogTemplate = "{NewLine}时间：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}日志级别：{Level}{NewLine}类别：{SourceContext}{NewLine}线程ID：{ThreadId}{NewLine}事件ID：{EventId}{NewLine}消息：{Message:lj}{NewLine}" + new string('-', 50);

        string errorLogTemplate = @"{NewLine}时间：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}日志级别：{Level}{NewLine}类别：{SourceContext}{NewLine}程ID：{ThreadId}{NewLine}事件ID：{EventId}{NewLine}异常消息：{Exception}{NewLine}" + new string('-', 50);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("EventId", Guid.NewGuid())
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
                .WriteTo.Console(outputTemplate: errorLogTemplate)
                .WriteTo.File(
                    GetLogFilePath("Error"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: errorLogTemplate)
            )
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
                .WriteTo.Console(outputTemplate: infoLogTemplate)
                .WriteTo.File(
                    GetLogFilePath("Information"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: infoLogTemplate)
            )
            .CreateLogger();

        return hostBuilder.UseSerilog();
    }

    private static string GetLogFilePath(string logLevel)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("文件路径被获取");
        Console.ForegroundColor = ConsoleColor.Gray;
        var logPath = Path.Combine("Logs", DateTime.Now.ToString("yyMM"), DateTime.Now.ToString("dd"));
        Directory.CreateDirectory(logPath);
        return Path.Combine(logPath, $"{logLevel}.log");
    }
}
