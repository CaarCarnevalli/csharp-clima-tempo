// Importações necessárias para os DTOs de país
using System.Collections.Generic;

// Define o namespace dos DTOs
namespace GloboClima.API.DTOs;

// DTO para moeda
public class CurrencyDto
{
    // Nome da moeda
    public string? Name { get; set; }

    // Símbolo da moeda
    public string? Symbol { get; set; }
}

// DTO para país
public class CountryDto
{
    // Nome comum do país
    public string? Name { get; set; }

    // Nome oficial do país
    public string? OfficialName { get; set; }

    // Capital do país
    public string? Capital { get; set; }

    // Região do país
    public string? Region { get; set; }

    // Países vizinhos
    public List<string>? Borders { get; set; }

    // URL da bandeira (SVG)
    public string? FlagUrl { get; set; }

    // Descrição da bandeira (alt text)
    public string? FlagDescription { get; set; }

    // População do país
    public long Population { get; set; }

    // Área do país
    public double Area { get; set; }

    // Idiomas falados no país (Ex: {"por": "Portuguese"})
    public Dictionary<string, string>? Languages { get; set; }

    // Moedas usadas no país (Ex: {"BRL": { Name: "Brazilian real", Symbol: "R$" }})
    public Dictionary<string, CurrencyDto>? Currencies { get; set; }

    // URL do Google Maps
    public string? GoogleMapsUrl { get; set; }

    // URL do brasão do país (SVG)
    public string? CoatOfArmsUrl { get; set; }

    // Adicione outras propriedades conforme necessário
}
