using Microsoft.AspNetCore.Http;

namespace WWC._240711.ASPNETCore.Auth;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        UserContext.SetContext(context.User.Identities.Select(p => p.Claims).FirstOrDefault()?.ToList() ?? default);
        await _next(context);
    }
}
