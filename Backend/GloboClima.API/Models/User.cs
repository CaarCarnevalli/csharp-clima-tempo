// Define o namespace do modelo
namespace GloboClima.API.Models;

// Define o modelo de usuário
public class User
{
    // ID do usuário
    public string? UserId { get; set; }

    // Nome de usuário
    public string? Username { get; set; }

    // Senha (em um cenário real, armazene o hash da senha)
    public string? Password { get; set; }

    // E-mail do usuário
    public string? Email { get; set; }
}
