using System.ComponentModel.DataAnnotations;

namespace GloboClima.Web.Models;

// DTO para a requisição de login.
public class LoginRequestDto
{
    // Nome de usuário para login. Campo obrigatório.
    [Required(ErrorMessage = "Usuário é obrigatório")]
    public string? Username { get; set; }

    // Senha para login. Campo obrigatório.
    [Required(ErrorMessage = "Senha é obrigatória")]
    public string? Password { get; set; }
}

// DTO para a requisição de registro de um novo usuário.
public class RegisterRequestDto
{
    // Nome de usuário desejado. Campo obrigatório.
    [Required(ErrorMessage = "Usuário é obrigatório")]
    public string? Username { get; set; }

    // Endereço de e-mail do usuário. Campo obrigatório e deve ser um e-mail válido.
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string? Email { get; set; }

    // Senha desejada para o novo usuário. Campo obrigatório.
    [Required(ErrorMessage = "Senha é obrigatória")]
    public string? Password { get; set; }
}

// DTO para a resposta da API após uma tentativa de login bem-sucedida.
public class LoginResponseDto
{
    // Token JWT para autenticação em requisições subsequentes.
    public string? Token { get; set; }
    // Data e hora de expiração do token.
    public DateTime Expiration { get; set; }
    // Nome de usuário do usuário autenticado.
    public string? Username { get; set; }
    // Identificador único do usuário autenticado.
    public string? UserId { get; set; }
    // Mensagem de erro, caso o login falhe (embora este DTO seja para respostas de sucesso, pode conter erros de validação específicos da API).
    public string? Error { get; set; } // Para mensagens de erro do login
} 