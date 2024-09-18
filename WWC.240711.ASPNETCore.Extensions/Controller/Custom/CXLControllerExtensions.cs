using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WWC._240711.ASPNETCore.Extensions.Filters.Custom;

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
                    var filterTypes = CXLFilterExtenions.GetCXLFilter();

                    foreach (var filter in filterTypes)
                    {
                        options.Filters.Add(filter);
                    }
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
