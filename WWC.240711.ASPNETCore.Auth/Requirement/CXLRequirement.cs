using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLRequirement : IAuthorizationRequirement
{
    public string PolicyName { get; }

    public CXLRequirement(string policyName)
    {
        PolicyName = policyName;
    }
}