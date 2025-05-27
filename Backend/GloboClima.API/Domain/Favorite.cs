// Importações necessárias para o modelo de favorito
using System;

// Define o namespace do modelo
namespace GloboClima.API.Domain;

// Define o modelo de favorito
public class Favorite
{
    // ID único do favorito (formato: UserId#CityName)
    public string? Id { get; set; }

    // ID do usuário
    public string? UserId { get; set; }

    // Nome da cidade
    public string? CityName { get; set; }

    // Nome do país
    public string? Country { get; set; }

    // Data de adição do favorito
    public DateTime DateAdded { get; set; }
}
