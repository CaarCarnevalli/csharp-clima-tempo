// Importações necessárias para o serviço de país
using GloboClima.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

// Define o namespace das interfaces
namespace GloboClima.API.Interfaces;

// Interface para o serviço de país
public interface ICountryService
{
    // Método para obter todos os países
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();

    // Método para obter um país pelo nome
    Task<CountryDto?> GetCountryByNameAsync(string name);
}
