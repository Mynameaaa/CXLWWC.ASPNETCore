using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WWC._240711.ASPNETCore.Extensions.WebHost.Custom.Applications
{
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder CustomMiddlewareSetup(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            //开发
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //预生产
            if (environment.IsStaging())
            {

            }

            //生产
            if (environment.IsProduction())
            {

            }

            var loggerProviders = app.ApplicationServices.GetServices<ILoggerProvider>();

            //app.Use(async (HttpContext context, RequestDelegate next) =>
            //{
            //    //context.Response.StatusCode = 401;
            //    //return;
            //});

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/swagger/{documentName}/swagger.json";
                //options.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.Use((HttpContext context, RequestDelegate next) =>
            //{
            //    throw new Exception("就是要异常");
            //});

            app.UseHttpsRedirection();

            app.UseRouting();

            ////中间件
            //app.UseCustomAllMiddleware();

            //app.UseAuthentication();

            app.UseAuthorization();

            //app.UseEndpoints(builder =>
            //{
            //    builder.MapControllers();
            //});

            //app.MapControllers();

            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
            });

            return app;
        }

    }
}
