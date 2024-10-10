using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WWC._240711.ASPNETCore.Extensions.Filters.Custom;

namespace WWC._240711.ASPNETCore.Production.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _logger.LogInformation("Very Good");
        }

        //[Authorize]
        [HttpGet(Name = "GetWeatherForecast")]
        //[TypeFilter<CXLExceptionFilter>]//不走依赖注入直接实例化
        //[ServiceFilter<CXLExceptionFilter>]//走依赖注入从容器中获取 Filter 实例
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
