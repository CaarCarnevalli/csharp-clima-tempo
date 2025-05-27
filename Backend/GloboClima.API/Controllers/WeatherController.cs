// Importações necessárias para o controlador de clima
using GloboClima.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// Define o namespace do controlador
namespace GloboClima.API.Controllers;

// Define o controlador de clima
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    // Serviço de clima
    private readonly IWeatherService _weatherService;

    // Construtor do controlador
    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    // Método para obter o clima de uma cidade
    [HttpGet("{city}")]
    public async Task<IActionResult> GetWeather(string city)
    {
        var weather = await _weatherService.GetWeatherAsync(city);
        if (weather == null)
        {
            return NotFound($"Weather data not found for city: {city}");
        }
        return Ok(weather);
    }
}
