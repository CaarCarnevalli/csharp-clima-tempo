// Importações necessárias para o serviço de clima
using GloboClima.API.DTOs;
using System.Threading.Tasks;

// Define o namespace das interfaces
namespace GloboClima.API.Interfaces;

// Interface para o serviço de clima
public interface IWeatherService
{
    // Método para obter o clima de uma cidade
    Task<WeatherDto?> GetWeatherAsync(string city);
}
