using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.TestAPI
{
    /// <summary>
    /// 添加额外控制器
    /// </summary>
    public class CXLControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // 查找所有实现了 ICXLController 接口的类
            var cxlControllerTypes = Assembly.GetEntryAssembly().GetTypes()
                .Where(t => typeof(ICXLController).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

            foreach (var type in cxlControllerTypes)
            {
                // 将这些类添加到 ASP.NET Core 控制器列表中
                feature.Controllers.Add(type.GetTypeInfo());
            }
        }
    }
}
