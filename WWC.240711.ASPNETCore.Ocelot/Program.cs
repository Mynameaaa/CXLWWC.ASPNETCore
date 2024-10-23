using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Errors.Middleware;
using System.Text;
using WWC._240711.ASPNETCore.Auth;
using WWC._240711.ASPNETCore.Auth.Extensions;
using WWC._240711.ASPNETCore.Extensions;
using WWC._240711.ASPNETCore.Extensions.Exceptions.Custom;
using WWC._240711.ASPNETCore.Extensions.Logging.Custom;
using WWC._240711.ASPNETCore.Infrastructure;
using WWC._240711.ASPNETCore.Ocelot;
using WWC._240711.ASPNETCore.Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddCXLSerilog();

builder.Services.AddSingleton(new Appsettings(builder.Configuration));

builder.Services.AddControllers();

//builder.Services.LoadProjectAssemblies();
builder.Services.AddAuthService();

builder.Services.AddCXLOcelot();

builder.Configuration.AddCXLOcelotConfigure();

builder.Services.AddCXLAuthorizationService();

builder.Services.AddCXLAuthentication();

var app = builder.Build();

//异常日志
app.UseMiddleware<CXLOcelotLastExecptionHandlerMiddleware>();
//正常日志
app.UseMiddleware<CXLOcelotLoggerRequestMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.UseCXLOcelot();

app.Run();
