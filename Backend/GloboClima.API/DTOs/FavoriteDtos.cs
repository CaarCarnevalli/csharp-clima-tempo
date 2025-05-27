// Define o namespace dos DTOs
namespace GloboClima.API.DTOs;

// DTO para favorito
public class FavoriteDto
{
    // ID do favorito
    public string? Id { get; set; }

    // ID do usuário
    public string? UserId { get; set; }

    // Nome da cidade
    public string? CityName { get; set; }

    // Nome do país
    public string? Country { get; set; }
}

// DTO para requisição de adição de favorito
public class AddFavoriteRequestDto
{
    // Nome da cidade
    public string? CityName { get; set; }

    // Nome do país
    public string? Country { get; set; }
}
