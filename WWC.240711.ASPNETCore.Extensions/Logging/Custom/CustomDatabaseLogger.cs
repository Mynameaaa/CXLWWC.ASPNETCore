using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Logging.Custom.Db;

namespace WWC._240711.ASPNETCore.Extensions.Logging.Custom
{
    public class CustomLogger : ILogger
    {
        private readonly LoggerDbContext _dbContext;
        public CustomLogger(LoggerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true; // 可以根据需要调整日志级别过滤
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            var logEntry = new CustomLogEntry
            {
                Message = message,
                Time = DateTime.Now,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Data = JsonConvert.SerializeObject(state),
            };

            //// 将日志条目格式化为 JSON 或其他所需格式
            //var logOutput = JsonSerializer.Serialize(logEntry);

            //// 在此处输出日志（例如，控制台，文件，数据库等）
            //Console.WriteLine(logOutput);

            Task.Run(async () =>
            {
                if (_dbContext != null)
                {
                    await _dbContext.CustomLogEntrys.AddAsync(logEntry);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var originalColor = Console.ForegroundColor;

                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        await Console.Out.WriteLineAsync($"{this.GetType().FullName ?? "CustomLogger"} DbContext 为空");
                    }
                    finally
                    {
                        Console.ForegroundColor = originalColor;
                    }
                }
            });
        }

        private string FormatState<TState>(TState state)
        {
            if (state is IReadOnlyList<KeyValuePair<string, object>> stateList)
            {
                var additionalData = new Dictionary<string, object>();

                foreach (var item in stateList)
                {
                    if (item.Key != "{OriginalFormat}")
                    {
                        additionalData[item.Key] = item.Value;
                    }
                }

                return JsonConvert.SerializeObject(additionalData);
            }

            return string.Empty;
        }
    }
}
