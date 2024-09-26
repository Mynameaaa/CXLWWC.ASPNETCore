using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class CXLAuthPolicyAttribute : Attribute
{
    public string[] Policy { get; set; }

    public CXLAuthPolicyAttribute(params string[] policyName)
    {
        // 设置基类的 Policy 属性，将策略名称传递给 IAuthorizationPolicyProvider
        Policy = policyName;
    }
}