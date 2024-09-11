using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.TestAPI
{
    public class CXLRouteConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                // 检查是否实现了 ICXLController 接口
                if (typeof(ICXLController).IsAssignableFrom(controller.ControllerType))
                {
                    // 处理类上的 CXLRoute 特性
                    var classRouteAttr = controller.ControllerType.GetCustomAttributes(true)
                        .OfType<CXLRouteAttribute>().FirstOrDefault();

                    if (classRouteAttr != null)
                    {
                        controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = classRouteAttr.Route,
                            Name = classRouteAttr.ApiName
                        };
                    }

                    // 处理方法上的 CXLRoute 特性
                    foreach (var action in controller.Actions)
                    {
                        var methodRouteAttr = action.ActionMethod.GetCustomAttributes(true)
                            .OfType<CXLRouteAttribute>().FirstOrDefault();

                        if (methodRouteAttr != null)
                        {
                            action.Selectors[0].AttributeRouteModel = new AttributeRouteModel
                            {
                                Template = methodRouteAttr.Route
                            };
                        }
                    }
                }
            }
        }
    }
}
