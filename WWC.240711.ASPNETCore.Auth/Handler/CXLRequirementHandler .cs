using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLRequirementHandler : AuthorizationHandler<CXLRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CXLRequirement requirement)
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
                Console.WriteLine(claim.Issuer + "----" + claim.Value);
            }
        }

        return Task.CompletedTask;
    }
}