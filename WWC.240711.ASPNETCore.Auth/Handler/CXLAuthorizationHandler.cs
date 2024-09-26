using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WWC._240711.ASPNETCore.Auth;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizationHandler : AuthorizationHandler<CXLPermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CXLPermissionRequirement requirement)
    {
        // 这里生日信息可以从其他地方获取，如数据库，不限于声明
        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);

        if (dateOfBirthClaim is null)
        {
            return Task.CompletedTask;
        }

        var today = DateTime.Today;
        var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);
        int calculatedAge = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-calculatedAge))
        {
            calculatedAge--;
        }

        // 若年龄达到最小年龄要求，则授权通过
        if (calculatedAge >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}