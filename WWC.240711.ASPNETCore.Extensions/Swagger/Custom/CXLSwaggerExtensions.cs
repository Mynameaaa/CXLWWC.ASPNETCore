using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Swagger
{
    public static class CXLSwaggerExtensions
    {
        public static IServiceCollection AddCXLSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(o =>
            {
                foreach (var group in typeof(CXLSwaggerGroup).GetFields().Where(f => f.Name != "value__"))
                {
                    var docName = group.Name;

                    o.SwaggerDoc(docName, new OpenApiInfo()
                    {
                        Title = docName + "标题",
                        Version = "v1.0.1",
                        Description = docName + "模块文档",
                        Contact = new OpenApiContact()
                        {
                            Email = "2244025263@qq.com",
                            //Extensions = exntension,
                            Name = "CXL---Contact",
                            Url = new Uri("http://localhost:5221/CXLdoc/Index.html?urls.primaryName=Stock")
                        },
                        License = new OpenApiLicense()
                        {
                            Name = "CXL---License",
                            Url = new Uri("http://localhost:5221/CXLdoc/Index.html?urls.primaryName=Order")
                        },
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            { "powered by", new Microsoft.OpenApi.Any.OpenApiString(".net8.0") }
                        },
                    });
                }

                //加载 xml 文件
                string basePath = AppContext.BaseDirectory;
                o.IncludeXmlComments(Path.Combine(basePath, "WWC.240711.ASPNETCore.Production.xml"));

                o.IncludeXmlComments(Path.Combine(basePath, "WWC.240711.ASPNETCore.Extensions.xml"));

                //操作级别过滤器，每个控制器都会调用一次
                o.OperationFilter<CXLSecurityOperationFilter>();
                o.OperationFilter<CXLSwaggerGroupOperationFilter>();

                //文档级别过滤器，每个文档会调用一次
                o.DocumentFilter<CXLSecurityDocumentFilter>();

                //Schema 级别过滤器
                o.SchemaFilter<CXLEnumSchemaFilter>();

                //忽略标记了 Obsolete 特性的方法
                o.SwaggerGeneratorOptions.IgnoreObsoleteActions = true;
                //忽略标记了 Obsolete 特性的属性
                o.SchemaGeneratorOptions.IgnoreObsoleteProperties = true;

                o.AddServer(new OpenApiServer()
                {
                    Description = "加密通信地址",
                    Url = "https://localhost:7889"
                });
                o.AddServer(new OpenApiServer()
                {
                    Description = "不安全的地址",
                    Url = "http://localhost:5231"
                });

            }).AddSwaggerGenNewtonsoftSupport();
        }

        public static IApplicationBuilder UseCXLSwagger(this WebApplication app)
        {
            // 启用 Swagger
            return app.UseSwagger(options =>
            {
                options.RouteTemplate = "/CXLdoc/{documentName}/swagger.json";
                //options.SerializeAsV2 = true;
            })
            .UseSwaggerUI(options =>
            {
                // 强制禁用浏览器缓存
                options.DefaultModelsExpandDepth(-1);  // 禁止默认展开模型
                options.DisplayRequestDuration();  // 显示请求持续时间
                options.RoutePrefix = "CXLdoc";
                foreach (var group in typeof(CXLSwaggerGroup).GetFields().Where(f => f.Name != "value__"))
                {
                    var docName = group.Name;
                    options.SwaggerEndpoint($"/CXLdoc/{docName}/swagger.json", docName);
                }
            });

        }

    }
}
