using LetThereBeLight.Devices;
using LetThereBeLight.Services;
using LetThereBeLight.Services.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LetThereBeLightApi.Controllers
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
        private readonly IDicoveryService _networkService;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IDicoveryService networkService)
        {
            _logger = logger;
            _networkService = networkService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var a = _networkService.DiscoverDevices(5000);
            SmartBulb bulb = a[0];

            if (bulb != null)
            {
                Thread.Sleep(2000);
                bulb.ToggleOnOff();
                Thread.Sleep(2000);
                bulb.ToggleOnOff();
                Thread.Sleep(2000);
                bulb.ToggleOnOff();
                Thread.Sleep(2000);
                bulb.ToggleOnOff();
                
                //bulb.SendCommand(bulb, 530714617, "set_power", new dynamic[] { "on", "smooth", 500 });
                //222, 235, 52
                //int rgb = (251 * 65536) + (252 * 256) + 199;
                //int brightness = Math.Max(1, Math.Min(100, Math.Abs(5)));
                //bulb.SendCommand(bulb, 530714617, "set_bright", new dynamic[] { rgb, "smooth", 500 });
                //bulb.SendCommand(bulb, 530714617, "set_rgb", new dynamic[] { rgb, "smooth", 1000 });
                //bulb.SendCommand(bulb, 530714617, "set_power", new dynamic[] { "off", "smooth", 500 });
                //bulb.SendCommand(bulb, 530714617, "set_power", new dynamic[] { "on", "smooth", 500 });
                //bulb.SendCommand(bulb, 530714617, "set_power", new dynamic[] { "off", "smooth", 500 });
                //bulb.SendCommand(bulb, 530714617, "set_power", new dynamic[] { "on", "smooth", 500 });
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}