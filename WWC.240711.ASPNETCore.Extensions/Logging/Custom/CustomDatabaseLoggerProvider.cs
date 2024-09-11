using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Logging.Custom.Db;

namespace WWC._240711.ASPNETCore.Extensions.Logging.Custom
{
    public class CustomDatabaseLoggerProvider<T> : ILoggerProvider where T : LoggerDbContext, new()
    {
        private readonly LoggerDbContext _dbContext;

        public CustomDatabaseLoggerProvider()
        {
            _dbContext = new T();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(_dbContext);
        }

        public void Dispose()
        {
            // 释放资源
        }
    }
}
