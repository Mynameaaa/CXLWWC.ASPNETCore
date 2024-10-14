using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Exceptions.Custom
{
    public class CXLDeveloperExceptionPageFilter : IDeveloperPageExceptionFilter
    {
        private readonly ILogger<CXLDeveloperExceptionPageFilter> _logger;

        public CXLDeveloperExceptionPageFilter(ILogger<CXLDeveloperExceptionPageFilter> logger)
        {
            _logger = logger;
        }

        public async Task HandleExceptionAsync(ErrorContext errorContext, Func<ErrorContext, Task> next)
        {
            if (errorContext.Exception == null)
                await next(errorContext);

            _logger.LogError($"开发人员异常页过滤器捕获到异常，异常信息：{errorContext.Exception}");

            await next(errorContext);
        }
    }
}
