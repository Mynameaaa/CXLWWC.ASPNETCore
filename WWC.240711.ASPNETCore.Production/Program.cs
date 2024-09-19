using WWC._240711.ASPNETCore.Production.Controllers;
using Microsoft.AspNetCore.Mvc;
using WWC._240711.ASPNETCore.Extensions;
using WWC._240711.ASPNETCore.Extensions.Controller.Custom;
using WWC._240711.ASPNETCore.Extensions.Swagger;
using WWC._240711.ASPNETCore.Extensions.Options.Custom;

var builder = WebApplication.CreateBuilder();

//配置加载
builder.InitConfiguration();

//自定义配置文件
builder.Configuration.AddDefaultDeveJsonFile();
builder.Configuration.AddDefaultWebConfigFile();
//未实现
//builder.Configuration.AddDataBaseConfiguration("");

//限流
builder.Services.AddRateLimiterSetup();

//跨域
builder.Services.AddCXLDefaultCors();

//加载控制器
builder.Services.AddCXLControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCXLHttpClientOptions();

builder.Services.ConfigureCXLNamedHttpClient();

builder.Services.AddCXLSwagger();

var app = builder.Build();

app.UseCXLSwagger();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCXLCors();

app.UseRateLimiterSetup();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/test", ([FromServices] ILogger<WeatherForecastController> logger) =>
{
    EventId eventId = new EventId(666, "MyEventId");
    logger.LogWarning("Very Good");
});

await app.RunAsync();