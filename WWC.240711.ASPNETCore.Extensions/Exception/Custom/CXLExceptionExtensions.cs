using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Exception.Custom
{
    public static class CXLExceptionExtensions
    {

        /// <summary>
        /// 注册开发人员异常过滤器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCXLDeveloperExceptionPage(this IServiceCollection services)
        {
            return services.AddSingleton<IDeveloperPageExceptionFilter, CXLDeveloperExceptionPageFilter>();
        }

        /// <summary>
        /// 使用开发人员异常页
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication UseCXLExceptionPage(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            return app;
        }

        /// <summary>
        /// 使用全局异常处理器
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCXLExceptionHandler(this WebApplication app)
        {
            return app.UseExceptionHandler(happ =>
            {
                var loggerFactory = happ.ApplicationServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("CXLExceptionHandler");

                happ.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    logger.LogError($"Exception Handled: {exceptionHandlerPathFeature?.Error}");

                    var statusCode = StatusCodes.Status500InternalServerError;
                    var message = exceptionHandlerPathFeature?.Error?.Message;

                    if (exceptionHandlerPathFeature?.Error is NotImplementedException)
                    {
                        message = "俺未实现";
                        statusCode = StatusCodes.Status501NotImplemented;
                    }

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "text/html";

                    var htmlResponse = $@"
                    <!DOCTYPE html>
                    <html lang='zh-CN'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>出错了</title>
                        <style>
                            body {{
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                height: 100vh;
                                background-color: #f8d7da;
                                font-family: Arial, sans-serif;
                            }}
                            .error-container {{
                                text-align: center;
                                background: white;
                                padding: 20px;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='error-container'>
                            <h1 class='display-4 text-danger'>发生错误！</h1>
                            <p class='lead'>抱歉，发生了一个意外的错误。</p>
                            <hr>
                            <p>错误信息: <strong>{message}</strong></p>
                            <p>状态码: <strong>{statusCode}</strong></p>
                            <a href='/' style='text-decoration: none; color: white; background-color: blue; padding: 10px; border-radius: 5px;'>返回首页</a>
                        </div>
                    </body>
                    </html>";

                    await context.Response.WriteAsync(htmlResponse);
                });
            });
        }

        /// <summary>
        /// 处理 404 状态码的请求
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCXL404StatusCodePage(this WebApplication app)
        {
            return app.UseMiddleware<CXL404StatusCodeMiddleware>();
        }

    }
}
