using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.MinimalAPI.Custom
{
    public static class CXLMinimalAPIExtensions
    {

        /// <summary>
        /// 启用健康检查 API
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static RouteHandlerBuilder UseCXLHealthMinimalAPI(this WebApplication app)
        {
            app.MapGet("/api/health", ([FromServices] ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger(typeof(CXLMinimalAPIExtensions).FullName ?? typeof(CXLMinimalAPIExtensions).Name);
                logger.LogInformation("【/api/health 健康检查被调用】");

                return Results.Json(new
                {
                    success = true,
                    message = "健康检查成功！"
                });
            })
                .WithDisplayName("ApiHealth")
                .WithGroupName("health");

            return app.MapGet("/health", ([FromServices] ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger(typeof(CXLMinimalAPIExtensions).FullName ?? typeof(CXLMinimalAPIExtensions).Name);
                logger.LogInformation("【/health 健康检查被调用】");

                return Results.Json(new
                {
                    success = true,
                    message = "健康检查成功！"
                });
            })
                .WithDisplayName("Health")
                .WithGroupName("health");
        }

    }
}
