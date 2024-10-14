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

#region JWT ¼øÈ¨

builder.Services.AddCXLAuthentication();

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
