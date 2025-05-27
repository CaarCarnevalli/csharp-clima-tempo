// Importações necessárias para o controlador de autenticação
using GloboClima.API.DTOs;
using GloboClima.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

// Define o namespace do controlador
namespace GloboClima.API.Controllers;

// Define o controlador de autenticação
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Configuração da aplicação
    private readonly IConfiguration _configuration;

    // Lista estática de usuários para simulação
    private static readonly List<User> _users = new List<User>
    {
        new User { Username = "usuario1", Password = "Senha123", Email = "usuario1@exemplo.com", UserId = "1" },
        new User { Username = "usuario2", Password = "Senha123", Email = "usuario2@exemplo.com", UserId = "2" },
        new User { Username = "usuario3", Password = "Senha123", Email = "usuario3@exemplo.com", UserId = "3" }
    };

    // Construtor do controlador
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Método para login de usuário
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto loginDto)
    {
        // Verifica se o usuário existe e a senha está correta
        var user = _users.FirstOrDefault(u =>
            u.Username == loginDto.Username &&
            u.Password == loginDto.Password);

        if (user != null)
        {
            var token = GenerateJwtToken(user);
            return Ok(token);
        }

        return Unauthorized("Nome de usuário ou senha inválidos.");
    }

// Método para registro de novo usuário
[HttpPost("register")]
public IActionResult Register([FromBody] RegisterRequestDto registerDto)
{
    // Verifica se o usuário já existe
    if (_users.Any(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
    {
        return BadRequest("Nome de usuário ou e-mail já está em uso.");
    }

    // Cria novo usuário
    var newUser = new User
    {
        Username = registerDto.Username,
        Password = registerDto.Password, // Em produção, deve-se usar hash de senha
        Email = registerDto.Email,
        UserId = (_users.Count + 1).ToString()
    };

    _users.Add(newUser);

    // Gera token para o novo usuário
    var token = GenerateJwtToken(newUser);
    return Ok(token);
}

// Método para obter lista de usuários
[HttpGet("users")]
public IActionResult GetUsers()
{
    // Retorna apenas informações não sensíveis
    var userInfo = _users.Select(u => new { u.Username, u.Email, u.UserId }).ToList();
    return Ok(userInfo);
}

// Método para gerar token JWT
private LoginResponseDto GenerateJwtToken(User user)
{
    var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
    var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured.");
    var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured.");

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    // Define as claims do token
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? string.Empty),
        new Claim(ClaimTypes.NameIdentifier, user.UserId ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var expires = DateTime.UtcNow.AddHours(1);

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: expires,
        signingCredentials: credentials);

    var tokenHandler = new JwtSecurityTokenHandler();
    return new LoginResponseDto
    {
        Token = tokenHandler.WriteToken(token),
        Expiration = expires,
        Username = user.Username,
        UserId = user.UserId
    };
}
}
