using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizationAllRequirementHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirementFor in context.Requirements)
        {
            Console.WriteLine("Requirements：" + requirementFor.GetType());
        }

        foreach (var requirementFor in context.PendingRequirements)
        {
            Console.WriteLine("PendingRequirements：" + requirementFor.GetType());
        }

        foreach (var claimsIdentity in context.User.Identities)
        {
            foreach (var claim in claimsIdentity.Claims)
            {
                Console.WriteLine(claim.Type + "：" + claim.Value);
            }
        }

        var requirements = context.PendingRequirements;

        foreach (var item in requirements)
        {
            if (item is CXLPermissionRequirement)
            {
                //自定义业务逻辑

            }
            if (item is CXLPermissionRequirementDelegate)
            {
                //自定义业务逻辑

            }
        }
        return Task.CompletedTask;
    }
}