//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WWC._240711.ASPNETCore.Extensions.Logging.源码
//{
//    internal class CustomLoggerFactory : ILoggerFactory
//    {
//        // 用于单例化 Logger<>
//        private readonly Dictionary<string, CustomLogger> _loggers = new Dictionary<string, CustomLogger>(StringComparer.Ordinal);
//        // 存放 ILoggerProviderRegistrations
//        private readonly List<ProviderRegistration> _providerRegistrations = new List<ProviderRegistration>();
//        private readonly object _sync = new object();

//        public CustomLoggerFactory(IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions> options = null)
//        {
//            // ...

//            // 注册 ILoggerProviders
//            foreach (ILoggerProvider provider in providers)
//            {
//                AddProviderRegistration(provider, dispose: false);
//            }

//            // ...
//        }

//        public ILogger CreateLogger(string categoryName)
//        {
//            lock (_sync)
//            {
//                // 如果不存在，则 new
//                if (!_loggers.TryGetValue(categoryName, out Logger logger))
//                {
//                    logger = new Logger
//                    {
//                        Loggers = CreateLoggers(categoryName),
//                    };

//                    (logger.MessageLoggers, logger.ScopeLoggers) = ApplyFilters(logger.Loggers);

//                    // 单例化 Logger<>
//                    _loggers[categoryName] = logger;
//                }

//                return logger;
//            }
//        }

//        private void AddProviderRegistration(ILoggerProvider provider, bool dispose)
//        {
//            _providerRegistrations.Add(new ProviderRegistration
//            {
//                Provider = provider,
//                ShouldDispose = dispose
//            });
//        }

//        private LoggerInformation[] CreateLoggers(string categoryName)
//        {
//            var loggers = new LoggerInformation[_providerRegistrations.Count];
//            // 循环遍历所有 ILoggerProvider
//            for (int i = 0; i < _providerRegistrations.Count; i++)
//            {
//                loggers[i] = new LoggerInformation(_providerRegistrations[i].Provider, categoryName);
//            }
//            return loggers;
//        }
//    }
//}
