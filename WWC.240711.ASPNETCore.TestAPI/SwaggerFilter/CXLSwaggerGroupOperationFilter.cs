using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WWC._240711.ASPNETCore.TestAPI
{
    public class CXLSwaggerGroupOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 获取控制器上的 GroupName 特性
            var controllerGroupNames = context.MethodInfo.DeclaringType?
                .GetCustomAttributes<CXLApiExplorerSettingsAttribute>()
                .Select(attr => new { groupName = attr.GroupName, tagName = attr.TagName })
                .ToList();

            // 获取方法上的 GroupName 特性
            var methodGroupNames = context.MethodInfo
                .GetCustomAttributes<CXLApiExplorerSettingsAttribute>()
                .Select(attr => new { groupName = attr.GroupName, tagName = attr.TagName })
                .ToList();

            // 组装所有 GroupName
            var groupNames = methodGroupNames.Any() ? methodGroupNames.Select(p => p.groupName) : controllerGroupNames?.Select(p => p.groupName) ?? new[] { "Default" };

            // 获取控制器名称（作为前缀）
            var controllerName = context.MethodInfo.DeclaringType?.Name.Replace("Controller", "");

            //获取标签信息
            var tagNames = methodGroupNames.Any() ? methodGroupNames.Select(p => p.tagName) : controllerGroupNames?.Select(p => p.tagName) ?? new[] { controllerName };

            // 如果有 GroupName，并且与 DocumentName 匹配，才添加标签
            if (groupNames != null && groupNames.Any())
            {
                // 添加所有匹配的 GroupName 作为标签
                operation.Tags = tagNames.Select(p => new OpenApiTag() { Name = p }).ToList();
                context.ApiDescription.GroupName = groupNames.FirstOrDefault();
            }
            else
            {

                operation.Tags = new List<OpenApiTag>
                    {
                        new OpenApiTag() { Name = controllerName }
                    };
                context.ApiDescription.GroupName = "Default";
            }
        }
    }
}
