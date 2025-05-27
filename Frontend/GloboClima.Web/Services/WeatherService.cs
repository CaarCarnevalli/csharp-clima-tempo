// Importações necessárias para o serviço de clima
using GloboClima.Web.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

// Define o namespace do serviço
namespace GloboClima.Web.Services;

// Define o serviço de clima
public class WeatherService
{
    // Cliente HTTP
    private readonly HttpClient _httpClient;

    // Construtor do serviço
    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Método para obter o clima
    public async Task<WeatherDto?> GetWeatherAsync(string city)
    {
        try
        {
            // Obter o clima da cidade
            var response = await _httpClient.GetFromJsonAsync<WeatherDto?>($"api/Weather/{city}");
            if (response != null)
            {
                // Garantir que os valores numéricos não sejam nulos
                response.Temperature = response.Temperature;
                response.FeelsLike = response.FeelsLike;
                response.Humidity = response.Humidity;
                response.WindSpeed = response.WindSpeed;
            }
            return response;
        }
        catch (HttpRequestException ex)
        {
            // Log ou manipule o erro como preferir
            System.Console.WriteLine($"Erro ao buscar clima: {ex.Message}");
            return null;
        }
    }
}
