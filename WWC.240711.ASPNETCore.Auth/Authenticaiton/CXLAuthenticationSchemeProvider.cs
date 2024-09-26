using Microsoft.AspNetCore.Authentication;
using System.Collections.Concurrent;

namespace WWC._240711.ASPNETCore.Auth;

/// <summary>
/// 用于管理 Scheme 以及 Handler 的类型
/// </summary>
public class CXLAuthenticationSchemeProvider : IAuthenticationSchemeProvider
{
    private readonly ConcurrentDictionary<string, AuthenticationScheme> _schemes = new ConcurrentDictionary<string, AuthenticationScheme>();
    private AuthenticationScheme? _defaultAuthenticateScheme;
    private AuthenticationScheme? _defaultChallengeScheme;
    private AuthenticationScheme? _defaultForbidScheme;
    private AuthenticationScheme? _defaultSignInScheme;
    private AuthenticationScheme? _defaultSignOutScheme;

    public CXLAuthenticationSchemeProvider()
    {
        // 初始化默认的认证方案
        var defaultScheme = new AuthenticationScheme("DefaultScheme", "Default Scheme", typeof(CXLAuthenticationHandler));
        _schemes.TryAdd("DefaultScheme", defaultScheme);
        _defaultAuthenticateScheme = defaultScheme;
        _defaultChallengeScheme = defaultScheme;
        _defaultForbidScheme = defaultScheme;
        _defaultSignInScheme = defaultScheme;
        _defaultSignOutScheme = defaultScheme;
    }

    public Task<AuthenticationScheme?> GetDefaultAuthenticateSchemeAsync()
    {
        return Task.FromResult(_defaultAuthenticateScheme);
    }

    public Task<AuthenticationScheme?> GetDefaultChallengeSchemeAsync()
    {
        return Task.FromResult(_defaultChallengeScheme);
    }

    public Task<AuthenticationScheme?> GetDefaultForbidSchemeAsync()
    {
        return Task.FromResult(_defaultForbidScheme);
    }

    public Task<AuthenticationScheme?> GetDefaultSignInSchemeAsync()
    {
        return Task.FromResult(_defaultSignInScheme);
    }

    public Task<AuthenticationScheme?> GetDefaultSignOutSchemeAsync()
    {
        return Task.FromResult(_defaultSignOutScheme);
    }

    public Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        _schemes.TryGetValue(name, out var scheme);
        return Task.FromResult(scheme);
    }

    public Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
    {
        return Task.FromResult<IEnumerable<AuthenticationScheme>>(_schemes.Values.ToList());
    }

    public Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
    {
        var requestHandlerSchemes = _schemes.Values.Where(s => typeof(IAuthenticationRequestHandler).IsAssignableFrom(s.HandlerType));
        return Task.FromResult(requestHandlerSchemes);
    }

    public void AddScheme(AuthenticationScheme scheme)
    {
        _schemes.TryAdd(scheme.Name, scheme);
    }

    public void RemoveScheme(string name)
    {
        _schemes.TryRemove(name, out _);
    }

    public bool TryAddScheme(AuthenticationScheme scheme)
    {
        return _schemes.TryAdd(scheme.Name, scheme);
    }

    // 添加用于设置默认方案的方法
    public void SetDefaultAuthenticateScheme(AuthenticationScheme scheme)
    {
        _defaultAuthenticateScheme = scheme;
    }

    public void SetDefaultChallengeScheme(AuthenticationScheme scheme)
    {
        _defaultChallengeScheme = scheme;
    }

    public void SetDefaultForbidScheme(AuthenticationScheme scheme)
    {
        _defaultForbidScheme = scheme;
    }

    public void SetDefaultSignInScheme(AuthenticationScheme scheme)
    {
        _defaultSignInScheme = scheme;
    }

    public void SetDefaultSignOutScheme(AuthenticationScheme scheme)
    {
        _defaultSignOutScheme = scheme;
    }
}
