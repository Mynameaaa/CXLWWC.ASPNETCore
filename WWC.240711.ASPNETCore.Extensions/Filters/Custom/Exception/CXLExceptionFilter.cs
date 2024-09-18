using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Filters.Custom
{
    public class CXLExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<CXLExceptionFilter> _logger;

        public CXLExceptionFilter(ILogger<CXLExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            Console.WriteLine("--------- Exception Filter ---------");
            Console.WriteLine("--------- 控制器方法调用异常 ---------");
            _logger.LogError("控制器方法执行过程的异常：" + context.Exception.ToString());
            context.Result = new JsonResult(new
            {
                Exception = context.Exception.ToString(),
                Code = 500,
                Message = "服务器异常！"
            });
            await Task.CompletedTask;
        }
    }
}
