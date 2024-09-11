//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WWC._240711.ASPNETCore.Extensions.Logging.源码
//{
//    public static class CustomAddLogging
//    {
//        public static IServiceCollection AddLoggingExtensions(this IServiceCollection services, Action<ILoggingBuilder> configure)
//        {
//            services.AddOptions();

//            // 注册单例 ILoggerFactory
//            services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
//            // 注册单例 ILogger<>
//            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

//            // 批量注册单例 IConfigureOptions<LoggerFilterOptions>
//            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>>(
//                new DefaultLoggerLevelConfigureOptions(LogLevel.Information)));

//            configure(new LoggingBuilder(services));
//            return services;
//        }

//    }
//}
