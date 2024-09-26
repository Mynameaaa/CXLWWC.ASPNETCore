using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Auth;

public static class CXLAuthenticationExtensions
{
    public static AuthenticationBuilder AddCXLAuthentication(this IServiceCollection services, Action<AuthenticationOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        var builder = services.AddAuthentication();

        builder.AddDefaultScheme();
        services.Configure<AuthenticationOptions>(options => options.DefaultScheme = CXLConstantScheme.DefaultScheme);
        services.Configure(configureOptions);
        services.AddCXLAuthenticationCore();
        return builder;
    }

    private static AuthenticationBuilder AddDefaultScheme(this AuthenticationBuilder builder)
    {
        string Issuer = Appsettings.app("JWT:Issuer") ?? "DefualtIssuer";
        string Audience = Appsettings.app("JWT:Audience") ?? "DefualtAudience";
        byte[] SecreityBytes = Encoding.UTF8.GetBytes(Appsettings.app("JWT:SecretKey") ?? Guid.NewGuid().ToString("N"));
        SecurityKey securityKey = new SymmetricSecurityKey(SecreityBytes);

        var defualtOptions = new CXLAuthenticationSchemeOptions()
        {
            Age = 18,
            DisplayName = "ZWJ",
            Issuer = Issuer,
            ValidateAudience = true,
            Audience = Audience,
            ValidateIssuer = true,
            SecretKey = securityKey,
            DefualtChallageMessage = "无效的 Token 或未找到合适的 Token",
            RedirectUrl = "https://google.com",
        };
        builder.AddScheme<CXLAuthenticationSchemeOptions, CXLAuthenticationHandler>(CXLConstantScheme.DefaultScheme, options => options = defualtOptions);

        return builder;
    }

    private static IServiceCollection AddCXLAuthenticationCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<ITokenHelper, TokenHelper>();
        //鉴权服务
        services.AddScoped<IAuthenticationService, CXLAuthenticationService>();
        //与用户身份主体相关
        services.TryAddSingleton<IClaimsTransformation, NoopClaimsTransformation>(); // Can be replaced with scoped ones that use DbContext
        //鉴权处理器提供者
        services.AddScoped<IAuthenticationHandlerProvider, CXLAuthenticationHandlerProvider>();
        //鉴权策略提供者
        services.TryAddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();

        return services;
    }
}
