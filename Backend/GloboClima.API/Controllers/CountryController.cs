// Importações necessárias para o controlador de países
using GloboClima.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// Define o namespace do controlador
namespace GloboClima.API.Controllers;

// Define o controlador de países
[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    // Serviço de países
    private readonly ICountryService _countryService;

    // Construtor do controlador
    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    // Método para obter países
    [HttpGet]
    public async Task<IActionResult> GetCountries([FromQuery] string? name = null)
    {
        // Verifica se o nome do país foi fornecido
        if (!string.IsNullOrEmpty(name))
        {
            var country = await _countryService.GetCountryByNameAsync(name);
            if (country == null)
            {
                return NotFound($"Country data not found for: {name}");
            }
            return Ok(country);
        }
        else
        {
            // Retorna todos os países
            var countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries);
        }
    }
}
