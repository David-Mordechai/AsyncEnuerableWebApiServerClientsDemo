using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using static System.Threading.Tasks.Task;

namespace DotnetServer.Controllers
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
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async IAsyncEnumerable<WeatherForecast> Get([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
            {
                _logger.LogWarning("Operation was canceled...");
            });
            for (var index = 0; index < 10; index++)
            {
                // Simulate an asynchronous delay
                await Delay(1000, cancellationToken);

                var item = new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };
                _logger.LogInformation("Item was yield to client, index: {index}, item:  {item}", index, item);
                yield return item;
            }
        }
    }
}
