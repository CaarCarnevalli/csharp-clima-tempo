// Importações necessárias para o serviço de APIs externas
using GloboClima.API.DTOs;
using GloboClima.API.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

// Define o namespace do serviço
namespace GloboClima.API.Services;

// Define o serviço de APIs externas
public class ExternalApiServices : IWeatherService, ICountryService
{
    // Cliente HTTP para requisições
    private readonly HttpClient _httpClient;

    // Configuração da aplicação
    private readonly IConfiguration _configuration;

    // Chave da API de clima
    private readonly string _weatherApiKey;

    // URL base da API de clima
    private readonly string _weatherApiBaseUrl;

    // URL base da API de países
    private readonly string _restCountriesBaseUrl;

    // Construtor do serviço
    public ExternalApiServices(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _weatherApiKey = _configuration["WeatherApi:ApiKey"] ?? throw new InvalidOperationException("WeatherApi API Key not configured.");
        _weatherApiBaseUrl = _configuration["WeatherApi:BaseUrl"] ?? throw new InvalidOperationException("WeatherApi Base URL not configured.");
        _restCountriesBaseUrl = _configuration["RestCountries:BaseUrl"] ?? throw new InvalidOperationException("RestCountries Base URL not configured.");
    }

    // Método para obter o clima de uma cidade
    public async Task<WeatherDto?> GetWeatherAsync(string city)
    {
        var requestUrl = $"{_weatherApiBaseUrl}current.json?key={_weatherApiKey}&q={city}&lang=pt_br";
        try
        {
            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var jsonDocument = JsonDocument.Parse(jsonString);

                if (jsonDocument.RootElement.TryGetProperty("current", out var currentWeather) &&
                    jsonDocument.RootElement.TryGetProperty("location", out var location))
                {
                    return new WeatherDto
                    {
                        City = location.TryGetProperty("name", out var cityName) ? cityName.GetString() : null,
                        Country = location.TryGetProperty("country", out var countryName) ? countryName.GetString() : null,
                        Description = currentWeather.TryGetProperty("condition", out var condition) &&
                                      condition.TryGetProperty("text", out var text) ? text.GetString() : null,
                        Temperature = currentWeather.TryGetProperty("temp_c", out var tempC) ? tempC.GetDouble() : 0,
                        FeelsLike = currentWeather.TryGetProperty("feelslike_c", out var feelsLike) ? feelsLike.GetDouble() : 0,
                        Humidity = currentWeather.TryGetProperty("humidity", out var humidity) ? humidity.GetInt32() : 0,
                        WindSpeed = currentWeather.TryGetProperty("wind_kph", out var windSpeed) ? windSpeed.GetDouble() : 0,
                        IconUrl = currentWeather.TryGetProperty("condition", out var iconCondition) &&
                                 iconCondition.TryGetProperty("icon", out var icon) ?
                                 $"https:{icon.GetString()}" : null,
                        LocalTime = location.TryGetProperty("localtime", out var localTime) ? localTime.GetString() : null,
                        UvIndex = currentWeather.TryGetProperty("uv", out var uv) ? uv.GetDouble() : 0
                    };
                }
                Console.WriteLine($"Failed to parse weather data for {city}. JSON structure might be unexpected.");
                return null;
            }
            else
            {
                Console.WriteLine($"Error fetching weather for {city}. Status: {response.StatusCode}, URL: {requestUrl}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error fetching weather for {city}: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Parsing Error fetching weather for {city}: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error fetching weather for {city}: {ex.Message}");
            return null;
        }
    }

    // Método para obter informações de um país pelo nome
    public async Task<CountryDto?> GetCountryByNameAsync(string name)
    {
        try
        {
            var requestUrl = $"{_restCountriesBaseUrl}name/{Uri.EscapeDataString(name)}?fullText=true";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Country not found: {name}, URL: {requestUrl}");
                }
                else
                {
                    Console.WriteLine($"Error fetching country info for {name}. Status: {response.StatusCode}, URL: {requestUrl}");
                }
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);

            if (jsonDoc.RootElement.ValueKind != JsonValueKind.Array || jsonDoc.RootElement.GetArrayLength() == 0)
            {
                 Console.WriteLine($"Unexpected response format or empty array for country: {name}");
                 return null;
            }

            var countryData = jsonDoc.RootElement[0];

            var languages = new Dictionary<string, string>();
            if (countryData.TryGetProperty("languages", out var languagesNode) && languagesNode.ValueKind == JsonValueKind.Object)
            {
                foreach (var langProp in languagesNode.EnumerateObject())
                {
                    languages[langProp.Name] = langProp.Value.GetString() ?? string.Empty;
                }
            }

            var currencies = new Dictionary<string, CurrencyDto>();
            if (countryData.TryGetProperty("currencies", out var currenciesNode) && currenciesNode.ValueKind == JsonValueKind.Object)
            {
                foreach (var currencyProp in currenciesNode.EnumerateObject())
                {
                    if (currencyProp.Value.TryGetProperty("name", out var currencyNameElement) &&
                        currencyProp.Value.TryGetProperty("symbol", out var currencySymbolElement))
                    {
                        currencies[currencyProp.Name] = new CurrencyDto
                        {
                            Name = currencyNameElement.GetString(),
                            Symbol = currencySymbolElement.GetString()
                        };
                    }
                }
            }

            var countryDto = new CountryDto
            {
                Name = countryData.TryGetProperty("name", out var nameObj) && nameObj.TryGetProperty("common", out var commonName) ? commonName.GetString() : name,
                OfficialName = nameObj.TryGetProperty("official", out var officialName) ? officialName.GetString() : null,
                Capital = countryData.TryGetProperty("capital", out var capArray) && capArray.ValueKind == JsonValueKind.Array && capArray.GetArrayLength() > 0 ? capArray[0].GetString() : null,
                Region = countryData.TryGetProperty("region", out var region) ? region.GetString() : null,
                Borders = countryData.TryGetProperty("borders", out var bordersArray) && bordersArray.ValueKind == JsonValueKind.Array
                            ? bordersArray.EnumerateArray().Select(b => b.GetString() ?? string.Empty).ToList()
                            : new List<string>(),
                FlagUrl = countryData.TryGetProperty("flags", out var flagsObj) && flagsObj.TryGetProperty("svg", out var svgUrl) ? svgUrl.GetString() : null,
                FlagDescription = flagsObj.TryGetProperty("alt", out var altText) ? altText.GetString() : null,
                Population = countryData.TryGetProperty("population", out var pop) ? pop.GetInt64() : 0,
                Area = countryData.TryGetProperty("area", out var area) ? area.GetDouble() : 0,
                Languages = languages,
                Currencies = currencies,
                GoogleMapsUrl = countryData.TryGetProperty("maps", out var mapsObj) && mapsObj.TryGetProperty("googleMaps", out var gMaps) ? gMaps.GetString() : null,
                CoatOfArmsUrl = countryData.TryGetProperty("coatOfArms", out var coaObj) && coaObj.TryGetProperty("svg", out var coaSvg) ? coaSvg.GetString() : null
            };

            return countryDto;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error fetching country info for {name}: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Parsing Error fetching country info for {name}: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error fetching country info for {name}: {ex.Message}");
            return null;
        }
    }

    // Método para obter informações de todos os países
    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
    {
        try
        {
            var requestUrl = $"{_restCountriesBaseUrl}all?fields=name,capital,region,flags,population,area,languages,currencies,maps,coatOfArms,borders";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching all countries. Status: {response.StatusCode}, URL: {requestUrl}");
                return Enumerable.Empty<CountryDto>();
            }

            var countriesJson = await response.Content.ReadAsStringAsync();
            var countriesList = JsonSerializer.Deserialize<List<JsonElement>>(countriesJson);

            if (countriesList == null) return Enumerable.Empty<CountryDto>();

            return countriesList.Select(countryData => {
                var languages = new Dictionary<string, string>();
                if (countryData.TryGetProperty("languages", out var languagesNode) && languagesNode.ValueKind == JsonValueKind.Object)
                {
                    foreach (var langProp in languagesNode.EnumerateObject())
                    {
                        languages[langProp.Name] = langProp.Value.GetString() ?? string.Empty;
                    }
                }

                var currencies = new Dictionary<string, CurrencyDto>();
                if (countryData.TryGetProperty("currencies", out var currenciesNode) && currenciesNode.ValueKind == JsonValueKind.Object)
                {
                    foreach (var currencyProp in currenciesNode.EnumerateObject())
                    {
                         if (currencyProp.Value.TryGetProperty("name", out var currencyNameElement) &&
                             currencyProp.Value.TryGetProperty("symbol", out var currencySymbolElement))
                        {
                            currencies[currencyProp.Name] = new CurrencyDto
                            {
                                Name = currencyNameElement.GetString(),
                                Symbol = currencySymbolElement.GetString()
                            };
                        }
                    }
                }

                return new CountryDto
                {
                    Name = countryData.TryGetProperty("name", out var nameObj) && nameObj.TryGetProperty("common", out var commonName) ? commonName.GetString() : null,
                    OfficialName = nameObj.TryGetProperty("official", out var officialName) ? officialName.GetString() : null,
                    Capital = countryData.TryGetProperty("capital", out var capArray) && capArray.ValueKind == JsonValueKind.Array && capArray.GetArrayLength() > 0 ? capArray[0].GetString() : null,
                    Region = countryData.TryGetProperty("region", out var region) ? region.GetString() : null,
                    Borders = countryData.TryGetProperty("borders", out var bordersArray) && bordersArray.ValueKind == JsonValueKind.Array
                            ? bordersArray.EnumerateArray().Select(b => b.GetString() ?? string.Empty).ToList()
                            : new List<string>(),
                    FlagUrl = countryData.TryGetProperty("flags", out var flagsObj) && flagsObj.TryGetProperty("svg", out var svgUrl) ? svgUrl.GetString() : null,
                    FlagDescription = flagsObj.TryGetProperty("alt", out var altText) ? altText.GetString() : null,
                    Population = countryData.TryGetProperty("population", out var pop) ? pop.GetInt64() : 0,
                    Area = countryData.TryGetProperty("area", out var area) ? area.GetDouble() : 0,
                    Languages = languages,
                    Currencies = currencies,
                    GoogleMapsUrl = countryData.TryGetProperty("maps", out var mapsObj) && mapsObj.TryGetProperty("googleMaps", out var gMaps) ? gMaps.GetString() : null,
                    CoatOfArmsUrl = countryData.TryGetProperty("coatOfArms", out var coaObj) && coaObj.TryGetProperty("svg", out var coaSvg) ? coaSvg.GetString() : null
                };
            }).ToList();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error fetching all countries: {ex.Message}");
            return Enumerable.Empty<CountryDto>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Parsing Error fetching all countries: {ex.Message}");
            return Enumerable.Empty<CountryDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error fetching all countries: {ex.Message}");
            return Enumerable.Empty<CountryDto>();
        }
    }
}
