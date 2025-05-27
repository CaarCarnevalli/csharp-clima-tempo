// Define o namespace dos DTOs
namespace GloboClima.API.DTOs;

// DTO para clima
public class WeatherDto
{
    // Nome da cidade
    public string? City { get; set; }

    // Nome do país
    public string? Country { get; set; }

    // Descrição do clima
    public string? Description { get; set; }

    // Temperatura em Celsius
    public double Temperature { get; set; }

    // Sensação térmica
    public double FeelsLike { get; set; }

    // Umidade
    public int Humidity { get; set; }

    // Velocidade do vento
    public double WindSpeed { get; set; }

    // URL do ícone do clima
    public string? IconUrl { get; set; }

    // Hora local
    public string? LocalTime { get; set; }

    // Índice UV
    public double UvIndex { get; set; }

    // Adicione outras propriedades conforme necessário
}
