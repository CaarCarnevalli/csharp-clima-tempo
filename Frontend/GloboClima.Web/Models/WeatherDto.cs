// Define o namespace para os modelos da aplicação web.
namespace GloboClima.Web.Models;

// Representa os dados de clima recebidos da API.
public class WeatherDto
{
    // Nome da cidade.
    public string? City { get; set; }
    // Código do país.
    public string? Country { get; set; }
    // Descrição do clima (ex: "Ensolarado", "Nublado").
    public string? Description { get; set; }
    // Temperatura em Celsius.
    public double Temperature { get; set; }
    // Sensação térmica em Celsius.
    public double FeelsLike { get; set; }
    // Umidade em porcentagem.
    public int Humidity { get; set; }
    // Velocidade do vento em km/h ou m/s (dependendo da API de origem).
    public double WindSpeed { get; set; }
    // URL para o ícone representativo do clima.
    public string? IconUrl { get; set; }
    // Horário local da cidade consultada (em formato string, ex: "2023-10-27T10:00:00").
    public string? LocalTime { get; set; }
    // Índice Ultravioleta (UV).
    public double UvIndex { get; set; }
} 