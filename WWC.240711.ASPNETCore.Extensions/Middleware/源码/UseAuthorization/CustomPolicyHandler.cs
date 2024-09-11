using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WWC._240711.ASPNETCore.Extensions.Middleware.源码.UseAuthorization
{
    public class CustomPolicyHandler : IPolicyEvaluator
    {
        public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            // 自定义身份验证逻辑
            // 这里可以添加任何您需要的逻辑来进行身份验证

            // 假设我们使用默认的身份验证处理程序
            var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();

            foreach (var scheme in await schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationHandler;
                if (handler != null)
                {
                    var result = await handler.AuthenticateAsync();
                    if (result?.Succeeded ?? false)
                    {
                        return result;
                    }
                }
            }

            // 如果没有找到有效的身份验证结果，可以返回未验证的结果
            return AuthenticateResult.NoResult();
        }

        public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            // 自定义授权逻辑
            // 这里可以添加任何您需要的逻辑来进行授权

            var authorizationService = context.RequestServices.GetRequiredService<IAuthorizationService>();
            var result = await authorizationService.AuthorizeAsync(context.User, resource, policy);
            return result.Succeeded
                ? PolicyAuthorizationResult.Success()
                : PolicyAuthorizationResult.Forbid();
        }
    }
}