// Importa o namespace para atributos de validação de dados.
using System.ComponentModel.DataAnnotations;

// Define o namespace para os modelos da aplicação web.
namespace GloboClima.Web.Models;

// Representa um item favorito, geralmente uma cidade.
public class FavoriteDto
{
    // Identificador único do favorito (geralmente UserId#CityName).
    public string? Id { get; set; }
    // Identificador do usuário ao qual este favorito pertence.
    public string? UserId { get; set; }
    // Nome da cidade favoritada.
    public string? CityName { get; set; }
    // País da cidade favoritada.
    public string? Country { get; set; }
}

// DTO para a requisição de adicionar um novo favorito.
public class AddFavoriteRequestDto
{
    // Nome da cidade a ser adicionada aos favoritos. Campo obrigatório.
    [Required(ErrorMessage = "Nome da cidade é obrigatório")]
    public string? CityName { get; set; }
    // País da cidade. Opcional, mas recomendado.
    public string? Country { get; set; }
}

// DTO para a resposta da API ao adicionar ou manipular um favorito.
public class FavoriteResponseDto
{
    // O objeto FavoriteDto, caso a operação seja bem-sucedida e retorne um favorito.
    public FavoriteDto? Favorite { get; set; }
    // Mensagem de erro, caso a operação falhe.
    public string? ErrorMessage { get; set; }
    // Indica se a operação da API foi bem-sucedida.
    public bool IsSuccess { get; set; }
    // Código de status HTTP da resposta da API.
    public int StatusCode { get; set; }
} 