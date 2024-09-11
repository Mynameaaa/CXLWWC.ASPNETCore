using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Logging
{
    public static class LoggingReview
    {
        public static WebApplicationBuilder CustomLogging(this WebApplicationBuilder builder)
        {
            //builder.Host.ConfigureLogging((context, logging) =>
            //{
            //    bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);


            //    if (isWindows)
            //    {
            //        logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
            //    }

            //});

            //添加日志相关服务
            builder.Services.AddLogging();

            //清除默认包含的提供程序
            builder.Logging.ClearProviders();

            //加载日志相关配置
            builder.Configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.logging.json"), false, true);

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            //添加 Logging 配置
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventSourceLogger();

            if (isWindows)
            {
                builder.Logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                // 在Windows平台上，添加 EventLogLoggerProvider
                builder.Logging.AddEventLog(settings =>
                {
                    settings.LogName = "应用程序名称";
                    settings.SourceName = "Runtime";
                    settings.MachineName = "本地计算机名称";
                });
            }

            //日志过滤器
            builder.Logging
                //设置最小日志级别
                .SetMinimumLevel(LogLevel.Trace)
                // 针对所有 LoggerProvider 设置 Microsoft 最小日志级别，建议通过配置文件进行配置
                .AddFilter("Microsoft", LogLevel.Trace)
                // 针对 ConsoleLoggerProvider 设置 Microsoft 最小日志级别，建议通过配置文件进行配置
                .AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Debug)
                // 针对所有 LoggerProvider 进行过滤配置
                .AddFilter((provider, category, logLevel) =>
                {
                    // 由于下面单独针对 ConsoleLoggerProvider 添加了过滤配置，所以 ConsoleLoggerProvider 不进入 该方法
                    if (provider == typeof(ConsoleLoggerProvider).FullName
                        && category == typeof(TestLoggingControl).FullName
                        && logLevel <= LogLevel.Warning)
                    {
                        // false：不记录日志
                        return false;
                    }

                    // true：记录日志
                    return true;
                }) // 针对 ConsoleLoggerProvider 进行过滤配置
                .AddFilter<ConsoleLoggerProvider>((category, logLevel) =>
                {
                    if (category == typeof(TestLoggingControl).FullName
                        && logLevel <= LogLevel.Warning)
                    {
                        // false：不记录日志
                        return false;
                    }

                    // true：记录日志
                    return true;
                });

            builder.Logging.Configure(options =>
            {
                options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                                    | ActivityTrackingOptions.TraceId
                                                    | ActivityTrackingOptions.ParentId;
            });

            //日志的输出格式配置
            builder.Logging
                .AddConsole()
                .AddSimpleConsole(options =>
                {
                    options.SingleLine = false;
                    options.ColorBehavior = LoggerColorBehavior.Default;
                })
                .AddSystemdConsole(options =>
                {

                })
                .AddJsonConsole(options =>
                {
                    options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions()
                    {
                        // 启用缩进
                        Indented = true,
                    };
                });

            return builder;
        }
    }
}
