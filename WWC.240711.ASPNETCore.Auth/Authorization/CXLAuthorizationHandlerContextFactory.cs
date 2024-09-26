using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WWC._240711.ASPNETCore.Auth;

// 摘要:
//      AuthorizationHandlerContext：
//      提供 AuthorizationHandlerContext 授权处理程序上下文用于授权
public class CXLAuthorizationHandlerContextFactory : IAuthorizationHandlerContextFactory
{
    //
    // 摘要:
    //      创建Microsoft。AspNetCore。授权。使用的授权处理程序上下文授权
    //
    // 参数:
    //      requirements:
    //      评估的要求。 AuthorizationRequirement
    //
    //      user:
    //      需要评估的用户票据.
    //
    //      resource:
    //      应检查策略的可选资源。如果资源不是策略评估所需的值可以为null。
    //
    // 返回结果:
    //     The Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext.
    public virtual AuthorizationHandlerContext CreateContext(IEnumerable<IAuthorizationRequirement> requirements, ClaimsPrincipal user, object? resource)
    {
        return new AuthorizationHandlerContext(requirements, user, resource);
    }
}