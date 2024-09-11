using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Extensions.Logging
{
    [CustomController]
    public class TestLoggingControl
    {
        private readonly ILogger<TestLoggingControl> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public TestLoggingControl(ILogger<TestLoggingControl> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public string TestLog()
        {
            _logger.LogInformation("打印日志");
            return "Success";
        }

        public string TestLogCreate()
        {
            var log = _loggerFactory.CreateLogger("CustomCategoryName");
            log.LogInformation("打印日志");
            return "Success";
        }

        public string TestLogTemplate(int ID)
        {
            _logger.LogInformation("打印日志 {id}", ID);
            _logger.LogInformation("Get {Id} at {Time}", DateTime.Now, ID);

            return "Success";
        }

    }
}
