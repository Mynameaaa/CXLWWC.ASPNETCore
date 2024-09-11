using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Middleware.Custom.Middleware;

namespace WWC._240711.ASPNETCore.Extensions.Middleware
{
    public static class MiddlewareReview
    {
        public static WebApplication UseCustomAllMiddleware(this WebApplication app)
        {
            //中间件基本形式
            app.Use(async (context, next) =>
            {
                //下游中间件执行前
                {
                    Console.WriteLine("Next Middleware No Starting");
                }
                await next(context);//执行下游中间件
                {
                    Console.WriteLine("Next Middleware Ended");
                }
                //下游中间件执行后
            });


            //添加请求头部中间件
            app.Use(async (context, next) =>
            {
                //在下游中间件前添加头部
                context.Request.Headers["TestMiddlewareAdd"] = "Abc";
                await next(context);
            });

            //中间件的几种编写方式
            app.Map("/Test", async context =>
            {
                context.Response.ContentType = "html/text";
                await context.Response.WriteAsync("测试");
            });

            app.MapWhen(options => options.Request.Path.HasValue && options.Request.Path.Value.Contains("Hello"), app =>
            {
                app.Run(async context =>
                {
                    context.Response.ContentType = "html/text";
                    await context.Response.WriteAsync("你好啊");
                });
            });

            //app.UseMiddleware(typeof(CustomExceptionHandlerMiddleware));
            //需要注意的是，这里的Middleware会自动注册为一个单例，所以在构造器注入时，
            //无法注入Scope生命周期的服务。如果注入，启动会直接报错

            //当我们需要注入Scope生命周期的服务时，直接在InvokeAsync方法中添加注入。
            app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            //需要注册，并且自定义生命周期
            app.UseMiddleware<CustomIMiddleware>();

            app.Use(next =>
            {
                //return 之前的代码是在启动时，初始化的时候执行
                Console.WriteLine("Use 中间件被初始化了。。。");
                return async context =>
                {
                    await next(context);
                };
            });

            //app.Run();

            return app;
        }
    }
}
