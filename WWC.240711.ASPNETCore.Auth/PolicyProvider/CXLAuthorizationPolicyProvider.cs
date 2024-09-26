using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using WWC._240711.ASPNETCore.Auth;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    // 默认的策略提供者，用于回退到默认的策略机制
    private DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public CXLAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        // 调用默认策略提供者
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        // 可选：为某些情况下返回备用策略
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        // 根据策略名称创建或返回对应的 AuthorizationPolicy
        if (policyName.StartsWith("CXL"))
        {
            // 根据策略名称生成 AuthorizationPolicy，例如
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new CXLRequirement(policyName)) // 自定义需求
                .Build();
            return Task.FromResult(policy);
        }

        // 如果策略名称不以 "CXL" 开头，则使用默认提供者
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}