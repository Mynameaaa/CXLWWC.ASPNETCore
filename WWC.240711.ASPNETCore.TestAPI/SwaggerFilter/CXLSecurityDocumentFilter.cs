using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WWC._240711.ASPNETCore.TestAPI
{
    /// <summary>
    /// 用于添加 JWT 鉴权的文档过滤器
    /// </summary>
    public class CXLSecurityDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // 增加全局的 JWT 鉴权配置
            swaggerDoc.Components.SecuritySchemes.Add("JwtBearer", new OpenApiSecurityScheme
            {
                Description = "这是方式一(直接在输入框中输入认证信息，不需要在开头添加Bearer)",
                Name = "Authorization", // JWT 默认的参数名称
                In = ParameterLocation.Header, // JWT 默认存放 Authorization 信息的位置(请求头中)
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            // 增加全局的 JWT 鉴权配置
            swaggerDoc.Components.SecuritySchemes.Add("CXLAuth", new OpenApiSecurityScheme
            {
                Description = "这是方式一(直接在输入框中输入认证信息，不需要在开头添加Bearer)",
                Name = "CXLToken", // JWT 默认的参数名称
                In = ParameterLocation.Header, // JWT 默认存放 Authorization 信息的位置(请求头中)
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

        }
    }
}
