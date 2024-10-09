using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WWC._240711.ASPNETCore.Auth;
using WWC._240711.ASPNETCore.Infrastructure;
using WWC._240711.ASPNETCore.Ocelot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new Appsettings(builder.Configuration));

builder.Services.AddControllers();

//builder.Services.LoadProjectAssemblies();

builder.Services.AddCXLOcelot();

builder.Configuration.AddCXLOcelotConfigure();

builder.Services.AddCXLAuthorizationService();

#region JWT 鉴权

builder.Services.AddCXLAuthentication(options =>
{
    options.DefaultScheme = CXLConstantScheme.Scheme;
    options.DefaultForbidScheme = CXLConstantScheme.Scheme;
    options.DefaultChallengeScheme = CXLConstantScheme.Scheme;
}).AddScheme<CXLAuthenticationSchemeOptions, CXLAuthenticationHandler>(CXLConstantScheme.Scheme, options =>
{
    if (Appsettings.app<bool>("UsePubPriKey"))
    {
        string privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PrivateKeyPath") ?? "Keys/private.pem");
        string publicKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PublicKeyPath") ?? "Keys/public.pem");

        KeyHelper keys = new KeyHelper();
        keys.GenerateKeys(privateKeyPath, publicKeyPath);

        // 使用生成的公钥来验证 JWT
        var publicKey = keys.LoadPublicKeyFromPEM(publicKeyPath);
        options.ValidateSecretKey = true;
        options.SecretKey = new RsaSecurityKey(publicKey);
        options.ValidateIssuer = false;
        options.ValidateAudience = false;
    }
    else
    {
        string Issuer = Appsettings.app("JWT: Issuer") ?? string.Empty;
        string Audience = Appsettings.app("JWT:Audience") ?? string.Empty;
        byte[] SecreityBytes = Encoding.UTF8.GetBytes(Appsettings.app("JWT:SecretKey") ?? string.Empty);
        SecurityKey securityKey = new SymmetricSecurityKey(SecreityBytes);

        options.Age = 18;
        options.DisplayName = "ZWJ";
        //发行人
        options.Issuer = Issuer;
        options.ValidateAudience = true;
        options.Audience = Audience;
        options.ValidateIssuer = true;
        options.SecretKey = securityKey;
        options.ValidateSecretKey = true;
        options.DefualtChallageMessage = "无效的 Token 或未找到合适的 Token";
        options.RedirectUrl = "https://google.com";
        ////自定义鉴权逻辑
        //options.AuthEvent += logger =>
        //{
        //    options.UseEventResult = true;
        //    return Task.FromResult(AuthenticateResult.Fail("你好"));
        //};
    }
});

#endregion

var app = builder.Build();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
    RequestPath = ""
});

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/api/auth/getToken",async ([FromServices] ITokenService _tokenService) =>
{
    TokenHelper tokenHelper = new TokenHelper();
    string privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PrivateKeyPath") ?? "Keys/public.pem");

    var tokenModel = new TokenDataModel()
    {
        Role = "Admin,User,BackAdmin",
        UserId = 666,
        Username = "ZWJJ"
    };

    Dictionary<string, string> dataModels = new Dictionary<string, string>
    {
        { "Name", tokenModel.Username },
        { "ID", tokenModel.UserId.ToString() },
        { "Role", tokenModel.Role }
    };

    return await _tokenService.GenerateJwtToken(privateKeyPath, dataModels);
});

app.UseMiddleware<UserContextMiddleware>();

app.MapControllers();

await app.UseCXLOcelot();

app.Run();
