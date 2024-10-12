using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Controller.Custom;
using WWC._240711.ASPNETCore.Extensions.Swagger;

namespace WWC._240711.ASPNETCore.Extensions.Program.Custom
{
    public class CXLExtensionsProgram
    {
        public async static Task MainAsync(WebApplicationBuilder builder)
        {
            //配置加载
            builder.InitConfiguration();

            //builder.AddCXLServiceContainer();

            //自定义配置文件
            builder.Configuration.AddDefaultDeveJsonFile();
            builder.Configuration.AddDefaultWebConfigFile();

            //限流
            builder.Services.AddRateLimiterSetup();

            //跨域
            builder.Services.AddCXLDefaultCors();

            //加载控制器 配置 Json
            builder.Services.AddCXLControllers();

            builder.Services.AddEndpointsApiExplorer();

            #region 未实现

            //builder.Services.AddCXLHttpClientOptions();
            //builder.Services.ConfigureCXLNamedHttpClient();
            //builder.Configuration.AddDataBaseConfiguration("");

            #endregion

            //加载 Swagger
            builder.Services.AddCXLSwagger();

            var app = builder.Build();

            app.UseCXLSwagger();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCXLCors();

            app.UseRateLimiterSetup();

            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/test", ([FromServices] ILogger<CXLExtensionsProgram> logger) =>
            {
                EventId eventId = new EventId(666, "MyEventId");
                logger.LogWarning("Very Good");
            });

            await app.RunAsync();

        }

        public static void Main(WebApplicationBuilder builder)
        {
            //配置加载
            builder.InitConfiguration();

            //builder.AddCXLServiceContainer();

            //自定义配置文件
            builder.Configuration.AddDefaultDeveJsonFile();
            builder.Configuration.AddDefaultWebConfigFile();

            //限流
            builder.Services.AddRateLimiterSetup();

            //跨域
            builder.Services.AddCXLDefaultCors();

            //加载控制器 配置 Json
            builder.Services.AddCXLControllers();

            builder.Services.AddEndpointsApiExplorer();

            #region 未实现

            //builder.Services.AddCXLHttpClientOptions();
            //builder.Services.ConfigureCXLNamedHttpClient();
            //builder.Configuration.AddDataBaseConfiguration("");

            #endregion

            //加载 Swagger
            builder.Services.AddCXLSwagger();

            var app = builder.Build();

            app.UseCXLSwagger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCXLCors();

            app.UseRateLimiterSetup();

            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/test", ([FromServices] ILogger<CXLExtensionsProgram> logger) =>
            {
                EventId eventId = new EventId(666, "MyEventId");
                logger.LogWarning("Very Good");
            });

            app.Run();

        }

    }
}
