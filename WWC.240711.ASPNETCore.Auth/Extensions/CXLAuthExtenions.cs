using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Auth.Extensions
{
    public static class CXLAuthExtensions
    {

        /// <summary>
        /// 添加授权相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthService(this IServiceCollection services)
        {
            services.AddSingleton<ITokenHelper, TokenHelper>();
            services.AddSingleton<IKeyHelper, KeyHelper>();
            return services.AddSingleton<ITokenService, TokenService>();
        }

    }
}
