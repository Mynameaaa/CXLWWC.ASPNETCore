using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WWC._240711.ASPNETCore.Extensions.Controller.Custom
{
    public static class CXLControllerExtensions
    {
        public static IMvcBuilder AddCXLControllers(this IServiceCollection builder)
        {
            return builder
                .AddControllers(options =>
                {
                    // 添加自定义的路由规则
                    options.Conventions.Add(new CXLRouteConvention());
                    options.Conventions.Add(new ApiExplorerHideOnlyConvention());
                })
                .ConfigureApplicationPartManager(manager =>
                {
                    // 添加自定义的 CXL 控制器提供程序
                    manager.FeatureProviders.Add(new CXLControllerFeatureProvider());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });
        }

    }
}
