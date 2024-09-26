using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthenticationService : IAuthenticationService
{
    /// <summary>
    /// 用于查找鉴权策略。
    /// </summary>
    public IAuthenticationSchemeProvider Schemes { get; }

    /// <summary>
    /// 用于解析鉴权处理器实例
    /// </summary>
    public IAuthenticationHandlerProvider Handlers { get; }

    /// <summary>
    /// Used for claims transformation.
    /// </summary>
    public IClaimsTransformation Transform { get; }

    /// <summary>
    /// The <see cref="AuthenticationOptions"/>.
    /// </summary>
    public AuthenticationOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="schemes">用于获取鉴权策略的<see cref="IAuthenticationSchemeProvider"/>.</param>
    /// <param name="handlers">用于获取鉴权处理器的<see cref="IAuthenticationHandlerProvider"/>.</param>
    /// <param name="transform">用户身份主体相关<see cref="IClaimsTransformation"/>.</param>
    /// <param name="options">鉴权策略相关<see cref="AuthenticationOptions"/>.</param>
    public CXLAuthenticationService(IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers, IClaimsTransformation transform, IOptions<AuthenticationOptions> options)
    {
        Schemes = schemes;
        Handlers = handlers;
        Transform = transform;
        Options = options.Value;
    }

    public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
    {
        var authenticationScheme = scheme ?? Options.DefaultAuthenticateScheme;
        if (string.IsNullOrEmpty(authenticationScheme))
        {
            return AuthenticateResult.NoResult();
        }

        var handler = await Handlers.GetHandlerAsync(context, authenticationScheme) as IAuthenticationHandler;
        if (handler == null)
        {
            return AuthenticateResult.NoResult();
        }

        return await handler.AuthenticateAsync();
    }

    public async Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
    {
        var authenticationScheme = scheme ?? Options.DefaultChallengeScheme;
        if (string.IsNullOrEmpty(authenticationScheme))
        {
            throw new InvalidOperationException("未找到 Challenge 的策略名称");
        }

        var handler = await Handlers.GetHandlerAsync(context, authenticationScheme);
        if (handler == null)
        {
            throw new InvalidOperationException($"策略 {authenticationScheme}，没有合适的 AuthenticationHandler");
        }

        await handler.ChallengeAsync(properties);
    }

    public async Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
    {
        var authenticationScheme = scheme ?? Options.DefaultForbidScheme;
        if (string.IsNullOrEmpty(authenticationScheme))
        {
            throw new InvalidOperationException("未找到 Forbid 的策略名称");
        }

        var handler = await Handlers.GetHandlerAsync(context, authenticationScheme);
        if (handler == null)
        {
            throw new InvalidOperationException($"策略 {authenticationScheme}，没有合适的 AuthenticationHandler");
        }

        await handler.ForbidAsync(properties);
    }

    public async Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
    {
        var authenticationScheme = scheme ?? Options.DefaultSignInScheme;
        if (string.IsNullOrEmpty(authenticationScheme))
        {
            throw new InvalidOperationException("未找到 SignIn 的策略名称");
        }

        var handler = await Handlers.GetHandlerAsync(context, authenticationScheme) as IAuthenticationSignInHandler;
        if (handler == null)
        {
            throw new InvalidOperationException($"策略 {authenticationScheme}，没有合适的 AuthenticationHandler");
        }

        await handler.SignInAsync(principal, properties);
    }

    public async Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
    {
        var authenticationScheme = scheme ?? Options.DefaultSignOutScheme;
        if (string.IsNullOrEmpty(authenticationScheme))
        {
            throw new InvalidOperationException("未找到 SignOut 的策略名称");
        }

        var handler = await Handlers.GetHandlerAsync(context, authenticationScheme) as IAuthenticationSignOutHandler;
        if (handler == null)
        {
            throw new InvalidOperationException($"策略 {authenticationScheme}，没有合适的 AuthenticationHandler");
        }

        await handler.SignOutAsync(properties);
    }
}
