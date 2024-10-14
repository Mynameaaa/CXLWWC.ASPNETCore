using WWC._240711.ASPNETCore.Auth;
using WWC._240711.ASPNETCore.Extensions.FileServer.Custom;
using WWC._240711.ASPNETCore.FileServer.Services;
using WWC._240711.ASPNETCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new Appsettings(builder.Configuration));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IDownLoadFileService, DownLoadFileService>();

#region …˙≥…√‹‘ø

string privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PrivateKeyPath") ?? "Keys/private.pem");
string publicKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PublicKeyPath") ?? "Keys/public.pem");

KeyHelper keys = new KeyHelper();
keys.GenerateKeys(privateKeyPath, publicKeyPath);

#endregion

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseCXLDefaultFiles();

app.UseCXLStaticFile("wwwroot", "wwwroot");

app.UseCXLStaticFile("Uploads", "Up");

app.UseCXLStaticFile("", "");

app.UseCXLDirectoryBrowser("FileServer", "FileRoot");

app.MapControllers();

app.Run();
