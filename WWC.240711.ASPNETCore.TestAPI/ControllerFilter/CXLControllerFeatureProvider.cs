using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.TestAPI
{
    /// <summary>
    /// 添加额外控制器
    /// </summary>
    public class CXLControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var assembliesList = Appsettings.app<string[]>("LoadControllersAeesmblies");
            if (assembliesList == null)
                return;

            List<Assembly> assemblies = new List<Assembly>();

            foreach (var assembly in assembliesList)
            {
                assemblies.Add(Assembly.Load(assembly));
            }

            // 查找所有实现了 ICXLController 接口的类
            var cxlControllerTypes = assemblies.SelectMany(p => p.GetTypes().Where(t => typeof(ICXLController).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass));

            foreach (var type in cxlControllerTypes)
            {
                // 将这些类添加到 ASP.NET Core 控制器列表中
                feature.Controllers.Add(type.GetTypeInfo());
            }
        }
    }
}
