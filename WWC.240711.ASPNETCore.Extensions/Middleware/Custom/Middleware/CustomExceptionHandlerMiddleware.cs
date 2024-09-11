using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Middleware
{
    /// <summary>
    /// 自定义异常处理中间件
    /// </summary>
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        //主机初始化时，就会实例化
        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine("自定义异常处理中间件捕捉到异常");
                await HandlerExecptionAsync(ex, httpContext);
            }
        }

        private async Task HandlerExecptionAsync(Exception exception, HttpContext httpContext)
        {
            Console.WriteLine(exception.Message);
        }

    }
}
