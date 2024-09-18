using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;

namespace WWC._240711.ASPNETCore.Extensions
{
    public static class CorsExtensions
    {
        private static List<CorsOptions> configureCorsOptions = new List<CorsOptions>();

        public static IServiceCollection AddPolicyCors(this IServiceCollection services, string policyName, Action<CorsOptions> optionsAction = null)
        {
            // 如果 UseCors 配置为 false 且没有提供额外的 Cors 配置操作，则不添加 CORS
            if (!Appsettings.app<bool>("UseCors") && optionsAction == null)
                return services;

            // 获取配置的 CorsOptions 列表
            var configureCorsOptions = new List<CorsOptions>();
            Appsettings.appSection("CorsOptions")?.Bind(configureCorsOptions);

            // 查找与 policyName 匹配的 CorsOptions
            var corsOptions = configureCorsOptions?.FirstOrDefault(p => p.PolicyName == policyName);

            // 如果找不到与 policyName 匹配的配置，且没有提供 optionsAction，则直接返回
            if (corsOptions == null && optionsAction == null)
                return services;

            // 如果配置为空，但有 optionsAction，初始化 corsOptions
            if (corsOptions == null)
                corsOptions = new CorsOptions();

            // 如果提供了外部的 optionsAction，则应用到 corsOptions 上
            optionsAction?.Invoke(corsOptions);

            //添加全部策略
            if (configureCorsOptions != null)
            {
                services.AddCors(options =>
                {
                    foreach (var configurePolicy in configureCorsOptions)
                    {
                        if (string.IsNullOrWhiteSpace(configurePolicy.PolicyName))
                        {
                            options.AddDefaultPolicy(policy =>
                            {
                                if (configurePolicy.AllowAnyOrigins)
                                    policy.AllowAnyOrigin();
                                else
                                    policy.WithOrigins(configurePolicy.WithOrigins ?? new string[] { });
                                if (configurePolicy.AllowAnyHeaders)
                                    policy.AllowAnyHeader();
                                else
                                    policy.WithOrigins(configurePolicy.WithHeaders ?? new string[] { });
                                if (configurePolicy.AllowAnyMethods)
                                    policy.AllowAnyOrigin();
                                else
                                    policy.WithOrigins(configurePolicy.WithMethods ?? new string[] { });
                            });
                        }
                        else
                        {
                            options.AddPolicy(configurePolicy.PolicyName, policy =>
                            {
                                if (configurePolicy.AllowAnyOrigins)
                                    policy.AllowAnyOrigin();
                                else
                                    policy.WithOrigins(configurePolicy.WithOrigins ?? new string[] { });
                                if (configurePolicy.AllowAnyHeaders)
                                    policy.AllowAnyHeader();
                                else
                                    policy.WithOrigins(configurePolicy.WithHeaders ?? new string[] { });
                                if (configurePolicy.AllowAnyMethods)
                                    policy.AllowAnyOrigin();
                                else
                                    policy.WithOrigins(configurePolicy.WithMethods ?? new string[] { });
                            });
                        }
                    }
                });
            }

            // 配置 UseCorsOptions 并设置策略名称(只有拥有策略的情况才会设置)
            services.Configure<UseCorsOptions>(options =>
            {
                options.OpenCors = true;
                options.PolicyName = corsOptions.PolicyName;
            });

            return services;
        }


        /// <summary>
        /// 使用默认策略
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCXLDefaultCors(this IServiceCollection services)
        {
            return services.AddPolicyCors("Defualt");
        }

        public static IServiceCollection AddCXLSystemCors(this IServiceCollection services)
        {
            return services.AddPolicyCors("System");
        }

        public static WebApplication UseCXLCors(this WebApplication app)
        {
            var useOptions = app.Services.GetService<IOptions<UseCorsOptions>>();

            if (useOptions == null)
                return app;

            var options = useOptions.Value;

            app.UseCors(options?.PolicyName ?? string.Empty);

            return app;
        }

    }
}
