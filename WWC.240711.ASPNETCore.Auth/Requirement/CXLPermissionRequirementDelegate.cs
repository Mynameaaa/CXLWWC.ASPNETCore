using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLPermissionRequirementDelegate : IAuthorizationRequirement
{
    public string UserName { get; init; }

    public string Work { get; init; }

    public string Address { get; init; }

    public string Remark { get; init; }

    public Func<CXLPermissionRequirementDelegate, bool> ValiationDelegate { get; init; }

    public CXLPermissionRequirementDelegate(Func<CXLPermissionRequirementDelegate, bool> valiationDelegate, string userName, string work, string address, string remark)
    {
        ValiationDelegate = valiationDelegate;
        UserName = userName;
        Work = work;
        Address = address;
        Remark = remark;
    }
}