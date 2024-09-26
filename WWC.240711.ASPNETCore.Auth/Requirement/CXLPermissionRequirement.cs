using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLPermissionRequirement : IAuthorizationRequirement
{
    public CXLPermissionRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }

    public int MinimumAge { get; set; }

}