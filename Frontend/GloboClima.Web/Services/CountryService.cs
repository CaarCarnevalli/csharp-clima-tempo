// Importações necessárias para o serviço de países
using GloboClima.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

// Define o namespace do serviço
namespace GloboClima.Web.Services;

// Define o serviço de países
public class CountryService
{
    // Cliente HTTP
    private readonly HttpClient _httpClient;

    // Construtor do serviço
    public CountryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Método para obter todos os países
    public async Task<IEnumerable<CountryDto>?> GetAllCountriesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CountryDto>?>("api/Country");
        }
        catch (HttpRequestException ex)
        {
            System.Console.WriteLine($"Erro ao buscar todos os países: {ex.Message}");
            return null;
        }
    }

    // Método para obter um país pelo nome
    public async Task<CountryDto?> GetCountryByNameAsync(string name)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CountryDto?>($"api/Country?name={name}");
        }
        catch (HttpRequestException ex)
        {
            System.Console.WriteLine($"Erro ao buscar país por nome: {ex.Message}");
            return null;
        }
    }
}
