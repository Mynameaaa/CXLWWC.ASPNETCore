using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Startup.使用
{
    internal class StartupFilterTwo : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                Console.WriteLine("Two");
                builder.Use(async (httpContext, _next) =>
                {
                    Console.WriteLine("-----StartupFilterTwo-----");
                    await _next(httpContext);
                });
                //相当于调用下一个 IStartupFilter 的 Configure 方法，如果不存在 IStartupFilter 了，则会调用 Startup 的Configure 方法
                next(builder);
            };
        }
    }
}
