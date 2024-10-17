using WWC._240711.ASPNETCore.Auth.Extensions;
using WWC._240711.ASPNETCore.AuthPlatform.Cache;
using WWC._240711.ASPNETCore.AuthPlatform.Servcies;
using WWC._240711.ASPNETCore.Extensions;
using WWC._240711.ASPNETCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new Appsettings(builder.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IDownLoadFileService, DownLoadFileService>();
builder.Services.AddTransient<IFileCacheService, FileCacheService>();

builder.Services.AddAuthService();

builder.Services.AddCXLHttpClient("FileServer", Appsettings.app("FileServerAPI"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
