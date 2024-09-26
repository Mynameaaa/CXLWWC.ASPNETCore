using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly IAuthorizationPolicyProvider _policyProvider;
    private readonly IAuthorizationService _authorizationService;
    private readonly IDictionary<string, string?> _propertiesItems = new Dictionary<string, string?>();

    public CXLAuthorizationFilter(IAuthorizationPolicyProvider policyProvider, IAuthorizationService authorizationService)
    {
        _policyProvider = policyProvider;
        _authorizationService = authorizationService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 获取自定义授权特性
        var customAuthAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<CXLAuthPolicyAttribute>().FirstOrDefault();

        if (customAuthAttribute != null)
        {
            if (customAuthAttribute.Policy == Array.Empty<string>())
                await Task.CompletedTask;

            foreach (var policyName in customAuthAttribute.Policy)
            {
                var policy = await _policyProvider.GetPolicyAsync(policyName);

                if (policy == null)
                    continue;

                var result = await _authorizationService.AuthorizeAsync(context.HttpContext.User, null, policy.Requirements);

                if (!result.Succeeded)
                {
                    _propertiesItems.Add(policyName, "授权失败");
                    context.Result = new ForbidResult(new AuthenticationProperties(_propertiesItems));
                }
            }
        }

        await Task.CompletedTask;
    }
}