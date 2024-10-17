using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text;
using WWC._240711.ASPNETCore.Auth.Cache;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Auth;

public static class CXLAuthenticationExtensions
{
    public static AuthenticationBuilder AddCXLAuthentication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CXLAuthenticationSchemeOptions.SchemeName;
        });
        if (Appsettings.app<bool?>("UsePubPriKey") ?? false)
        {
            builder.AddDefaultSchemeTwoKey();
        }
        else
        {
            builder.AddDefaultScheme();
        }

        services.AddCXLAuthenticationCore();
        return builder;
    }

    private static AuthenticationBuilder AddDefaultSchemeTwoKey(this AuthenticationBuilder builder)
    {
        //string privateKeyPath = "http://localhost:14670/api/auth/OAuth/privateKey";
        string publicKeyPath = "http://localhost:14670/api/auth/OAuth/publicKey";

        var _httpClient = new HttpClient();
        //byte[] privateFileBytes;
        byte[] publicFileBytes;
        var cache = new FileCacheService();

        if (cache.HasKey(CacheConstantKeys.TokenPrivateKey) && cache.HasKey(CacheConstantKeys.TokenPublicKey))
        {
            //privateFileBytes = cache.GetFile(CacheConstantKeys.TokenPrivateKey);
            publicFileBytes = cache.GetFile(CacheConstantKeys.TokenPublicKey);
        }
        else
        {
            //HttpResponseMessage privateKeyResponse = _httpClient.GetAsync(privateKeyPath).Result;
            //privateFileBytes = privateKeyResponse.Content.ReadAsByteArrayAsync().Result;
            HttpResponseMessage publicKeyResponse = _httpClient.GetAsync(publicKeyPath).Result;
            publicFileBytes = publicKeyResponse.Content.ReadAsByteArrayAsync().Result;
            cache.CacheFile(CacheConstantKeys.TokenPublicKey, publicFileBytes);
        }

        KeyHelper keys = new KeyHelper();

        // 使用生成的公钥来验证 JWT
        var publicKey = keys.LoadPublicKeyFromPEM(publicFileBytes);
        string Issuer = Appsettings.app("JWT:Issuer") ?? "DefualtIssuer";
        string Audience = Appsettings.app("JWT:Audience") ?? "DefualtAudience";

        var defaultOptions = new CXLAuthenticationSchemeOptions()
        {
            SecretKey = new RsaSecurityKey(publicKey),
            Age = 18,
            DisplayName = "ZWJ",
            Issuer = Issuer,
            ValidateAudience = true,
            Audience = Audience,
            ValidateIssuer = true,
            DefualtChallageMessage = "无效的 Token 或未找到合适的 Token",
            RedirectUrl = "https://google.com",
        };

        _httpClient.Dispose();
        return builder
            .AddScheme<CXLAuthenticationSchemeOptions, CXLAuthenticationHandler>(CXLAuthenticationSchemeOptions.SchemeName, options => options = defaultOptions);
    }

    private static AuthenticationBuilder AddDefaultScheme(this AuthenticationBuilder builder)
    {
        string Issuer = Appsettings.app("JWT:Issuer") ?? "DefualtIssuer";
        string Audience = Appsettings.app("JWT:Audience") ?? "DefualtAudience";
        byte[] SecreityBytes = Encoding.UTF8.GetBytes(Appsettings.app("JWT:SecretKey") ?? Guid.NewGuid().ToString("N"));
        SecurityKey securityKey = new SymmetricSecurityKey(SecreityBytes);

        var defaultOptions = new CXLAuthenticationSchemeOptions()
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
        return builder.AddScheme<CXLAuthenticationSchemeOptions, CXLAuthenticationHandler>(CXLAuthenticationSchemeOptions.SchemeName, options => options = defaultOptions);
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
