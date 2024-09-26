using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;

namespace WWC._240711.ASPNETCore.Auth;

public static class CXLAuthorizationExtensions
{
    public static IServiceCollection AddCXLAuthorizationService(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, CXLAuthorizationHandler>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, CXLAuthorizationDelegateHandler>());


        //注册一个处理多个 Requirement 的 IAuthorizationHandler 
        services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, CXLAuthorizationAllRequirementHandler>());

        services.AddAuthorization(options =>
        {
            //必须包含角色Claim
            options.AddPolicy("SystemRole", policy => policy.RequireRole("Role"));
            options.AddPolicy("CustomRole", policy => policy.RequireClaim(ClaimTypes.Role));

            //包含角色 Claim 且值必须为 Admin
            options.AddPolicy("SystemRoleValue", policy => policy.RequireRole("Admin"));
            options.AddPolicy("CustomRoleValue", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));

            //包含角色 Claim 且值必须为 Admin 或者 Agent
            options.AddPolicy("SystemRoleValueAnyOne", policy => policy.RequireRole("Admin", "Agent"));
            options.AddPolicy("CustomRoleValueAnyOne", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "Agent"));

            //包含角色 Claim 且值必须包含 Admin 和 Agent，集合形式才可以通过验证
            options.AddPolicy("CustomRoleValueAll", policy => policy.RequireClaim(ClaimTypes.Role, "Admin").RequireClaim(ClaimTypes.Role, "Agent"));
            options.AddPolicy("SystemRoleValueAll", policy => policy.RequireRole("Admin").RequireRole("Agent"));

            //包含 Age Claim
            options.AddPolicy("CustomAgeAndWork", policy => policy.RequireClaim("Age"));

            //包含 Age Claim 和 Work Claim
            options.AddPolicy("CustomAgeAndWork", policy => policy.RequireClaim("Age").RequireClaim("Work"));

            //包含 Age 并且值为 18
            options.AddPolicy("CustomAgeValue", policy => policy.RequireClaim("Age", "18"));

            //包含 Age 并且值为 18 或者 21
            options.AddPolicy("CustomAgeValueAnyOne", policy => policy.RequireClaim("Age", "18", "21"));

            //包含 StoreName 并且值为 Root 和 Admin
            options.AddPolicy("CustomAgeValueAnyOne", policy => policy.RequireClaim("StoreName", "Root").RequireClaim("Admin"));


            //包含角色 Claim 或者 名称 Claim
            options.AddPolicy("CustomRoleOrName", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(c => c.Type == ClaimTypes.Role) ||
                     context.User.HasClaim(c => c.Type == ClaimTypes.Name);
                });
            });

            //自定义简单策略授权
            options.AddPolicy("CustomValidationAge", policy => policy.Requirements.Add(new CXLPermissionRequirement(18)));
            options.AddPolicy("CustomDelegateValidation", policy => policy.Requirements.Add(new CXLPermissionRequirementDelegate(options =>
            {
                //自定义验证逻辑
                if (options.UserName.Equals("张无忌"))
                    return true;
                return false;
            }, "张无忌", "明教教主", "光明顶", "九阳神功与乾坤大挪移护体")));

        });

        return services;
    }

}
