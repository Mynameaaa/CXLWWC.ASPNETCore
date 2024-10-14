using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Exceptions.Custom
{
    /// <summary>
    /// 404 异常返回异常页
    /// </summary>
    public class CXL404StatusCodeMiddleware
    {
        private readonly RequestDelegate _next;

        public CXL404StatusCodeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                context.Response.ContentType = "text/html";
                var htmlResponse = @"
            <!DOCTYPE html>
            <html lang='zh-CN'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>页面未找到</title>
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
                    <h1 class='display-4 text-danger'>404 - 页面未找到</h1>
                    <p class='lead'>抱歉，您请求的页面不存在。</p>
                    <hr>
                    <p>请检查您输入的URL是否正确。</p>
                    <a href='/' style='text-decoration: none; color: white; background-color: blue; padding: 10px; border-radius: 5px;'>返回首页</a>
                </div>
            </body>
            </html>";

                await context.Response.WriteAsync(htmlResponse);
            }
        }
    }
}
