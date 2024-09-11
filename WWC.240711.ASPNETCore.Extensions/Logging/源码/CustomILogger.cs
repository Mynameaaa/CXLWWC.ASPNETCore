//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WWC._240711.ASPNETCore.Extensions.Logging.源码
//{
//    public interface CustomILogger
//    {
//        void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);

//        // 检查能否记录该日志等级的日志
//        bool IsEnabled(LogLevel logLevel);

//        IDisposable BeginScope<TState>(TState state);
//    }
//    public interface CustomILogger<out TCategoryName> : CustomILogger
//    {

//    }
//}
