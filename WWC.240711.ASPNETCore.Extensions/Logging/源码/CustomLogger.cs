//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WWC._240711.ASPNETCore.Extensions.Logging.源码
//{
//    internal class CustomLogger<T> : CustomILogger<T>
//    {
//        // 接口实现内部均是使用该实例进行操作
//        private readonly ILogger _logger;

//        // 果不其然，注入了 ILoggerFactory 实例
//        public CustomLogger(ILoggerFactory factory)
//        {
//            // 还记得吗？上面提到显式指定日志类别时，也是这样创建 ILogger 实例的
//            _logger = factory.CreateLogger(typeof(T).FullName);
//        }

//        public IDisposable BeginScope<TState>(TState state)
//        {
//            throw new NotImplementedException();
//        }

//        public bool IsEnabled(LogLevel logLevel)
//        {
//            throw new NotImplementedException();
//        }

//        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
//        {
//            throw new NotImplementedException();
//        }
//    }
//    internal class CustomLogger : CustomILogger
//    {
//        // 接口实现内部均是使用该实例进行操作
//        private readonly ILogger _logger;

//        // 果不其然，注入了 ILoggerFactory 实例
//        public CustomLogger(ILoggerFactory factory, string categoryName)
//        {
//            // 还记得吗？上面提到显式指定日志类别时，也是这样创建 ILogger 实例的
//            _logger = factory.CreateLogger(categoryName);
//        }
//        public IDisposable BeginScope<TState>(TState state)
//        {
//            throw new NotImplementedException();
//        }

//        public bool IsEnabled(LogLevel logLevel)
//        {
//            throw new NotImplementedException();
//        }

//        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
