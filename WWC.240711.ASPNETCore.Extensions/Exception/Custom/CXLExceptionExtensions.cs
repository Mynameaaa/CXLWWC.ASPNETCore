using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Exception.Custom
{
    public static class CXLExceptionExtensions
    {
        public static IServiceCollection AddCXLDeveloperExceptionPage(this IServiceCollection services)
        {
            return services.AddSingleton<IDeveloperPageExceptionFilter, CXLDeveloperExceptionPageFilter>();
        }

        public static WebApplication UseCXLExceptionPage(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            return app;
        }

    }
}
