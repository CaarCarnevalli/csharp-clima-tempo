using System.Collections.Generic;

namespace GloboClima.Web.Models;

// Representa os dados de uma moeda, incluindo nome e símbolo.
public class CurrencyDto // Novo DTO para Moeda
{
    // Nome da moeda (ex: "Real Brasileiro").
    public string? Name { get; set; }
    // Símbolo da moeda (ex: "R$").
    public string? Symbol { get; set; }
}

// Representa os dados detalhados de um país, recebidos da API.
public class CountryDto
{
    // Nome comum do país (ex: "Brasil").
    public string? Name { get; set; } // Common Name
    // Nome oficial do país (ex: "República Federativa do Brasil").
    public string? OfficialName { get; set; } // Official Name
    // Capital do país.
    public string? Capital { get; set; }
    // Região ou continente onde o país está localizado.
    public string? Region { get; set; }
    // Lista de códigos de países que fazem fronteira.
    public List<string>? Borders { get; set; }
    // URL para a imagem da bandeira do país.
    public string? FlagUrl { get; set; }
    // Descrição textual da bandeira (para acessibilidade).
    public string? FlagDescription { get; set; }
    // População total do país.
    public long Population { get; set; }
    // Área territorial do país em unidades quadradas (ex: km²).
    public double Area { get; set; }
    // Dicionário de idiomas falados no país (chave: código do idioma, valor: nome do idioma).
    public Dictionary<string, string>? Languages { get; set; } // Ex: {"por": "Portuguese"}
    // Dicionário de moedas utilizadas no país (chave: código da moeda, valor: objeto CurrencyDto).
    public Dictionary<string, CurrencyDto>? Currencies { get; set; } // Ex: {"BRL": { Name: "Brazilian real", Symbol: "R$" }}
    // URL para a localização do país no Google Maps.
    public string? GoogleMapsUrl { get; set; }
    // URL para a imagem do brasão de armas do país (geralmente em formato SVG).
    public string? CoatOfArmsUrl { get; set; } // SVG
} 