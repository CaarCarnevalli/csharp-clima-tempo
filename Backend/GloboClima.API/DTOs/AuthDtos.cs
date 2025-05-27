// Importações necessárias para os DTOs de autenticação
using System;

// Define o namespace dos DTOs
namespace GloboClima.API.DTOs;

// DTO para requisição de login
public class LoginRequestDto
{
    // Nome de usuário
    public string? Username { get; set; }

    // Senha
    public string? Password { get; set; }
}

// DTO para requisição de registro
public class RegisterRequestDto
{
    // Nome de usuário
    public string? Username { get; set; }

    // Senha
    public string? Password { get; set; }

    // E-mail
    public string? Email { get; set; }
}

// DTO para resposta de login
public class LoginResponseDto
{
    // Token JWT
    public string? Token { get; set; }

    // Data de expiração do token
    public DateTime Expiration { get; set; }

    // Nome de usuário
    public string? Username { get; set; }

    // ID do usuário
    public string? UserId { get; set; }
}
