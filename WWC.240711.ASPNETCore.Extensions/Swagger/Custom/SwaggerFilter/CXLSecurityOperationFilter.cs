using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WWC._240711.ASPNETCore.Extensions
{
    /// <summary>
    /// 用于标记带有 [Authorize] 特性的操作
    /// </summary>
    public class CXLSecurityOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 检查控制器是否标记了 [Authorize] 特性
            var hasControllerAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                        .OfType<AuthorizeAttribute>().Any();

            // 检查操作方法是否标记了 [Authorize] 特性
            var hasMethodAuthorize = context.MethodInfo.GetCustomAttributes(true)
                                     .OfType<AuthorizeAttribute>().Any();

            // 如果控制器或方法有 [Authorize] 特性，则添加锁图标和鉴权配置
            if (hasControllerAuthorize || hasMethodAuthorize)
            {
                // 添加 JWT 鉴权配置
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "JwtBearer"
                                }
                            },
                            new string[] { }
                        }
                    },
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "CXLAuth"
                                }
                            },
                            new string[] { }
                        }
                    }
                };

                // 添加锁图标，方便在 Swagger UI 中可视化
                if (operation.Tags != null && operation.Tags.Count > 0)
                {
                    foreach (var tag in operation.Tags)
                    {
                        tag.Name += " 🔒"; // 给标签添加锁图标
                    }
                }
            }
            else
            {
                // 确保没有 Authorize 特性时，不添加锁图标
                if (operation.Tags != null && operation.Tags.Count > 0)
                {
                    foreach (var tag in operation.Tags)
                    {
                        // 移除任何之前错误添加的锁图标
                        tag.Name = tag.Name.Replace(" 🔒", "");
                    }
                }
            }
        }
    }
}
