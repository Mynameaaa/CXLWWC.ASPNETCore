using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Middleware.源码.UseDeveloperExceptionPage
{
    /// <summary>
    /// 异常处理 Filter 可以定义多个会一个个执行
    /// </summary>
    public class CustomIDeveloperPageExceptionFilter : IDeveloperPageExceptionFilter
    {
        public CustomIDeveloperPageExceptionFilter()
        {
            Console.WriteLine("异常处理中间件初始化");
        }

        public async Task HandleExceptionAsync(ErrorContext errorContext, Func<ErrorContext, Task> next)
        {
            Console.WriteLine("异常处理中间件被调用");
            if (errorContext.Exception != null)
            {
                await next(errorContext);
                Console.WriteLine("异常 Message：" + errorContext.Exception.Message);
            }
            else
            {
            }
        }
    }
}
