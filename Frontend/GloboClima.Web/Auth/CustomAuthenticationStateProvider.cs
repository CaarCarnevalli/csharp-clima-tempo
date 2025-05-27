// Importações necessárias para o provedor de estado de autenticação personalizado.
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
// using System.Text.Json; // Comentado pois JsonSerializer não é usado diretamente aqui.
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic; // Para IEnumerable<Claim>

// Define o namespace para os componentes de autenticação da aplicação web.
namespace GloboClima.Web.Auth;

// Provedor de estado de autenticação personalizado que gerencia o estado de autenticação do usuário.
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    // Serviço para interagir com o armazenamento local do navegador.
    private readonly ILocalStorageService _localStorage;

    // Construtor que injeta o serviço de armazenamento local.
    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // Obtém o estado de autenticação atual do usuário.
    // Verifica se um token JWT existe no armazenamento local e cria um ClaimsPrincipal a partir dele.
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Tenta recuperar o token de autenticação do armazenamento local.
        var token = await _localStorage.GetItemAsync<string>("authToken");

        // Se o token não existir ou estiver vazio, retorna um estado de usuário não autenticado (anônimo).
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Se um token existir, parseia as claims do JWT.
        var claims = ParseClaimsFromJwt(token);
        // Cria um ClaimsPrincipal com as claims e o tipo de autenticação "jwt".
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        
        // Retorna o estado de autenticação com o usuário construído a partir do token.
        return new AuthenticationState(user);
    }

    // Notifica a aplicação que o usuário foi autenticado.
    // Este método é chamado após um login bem-sucedido.
    public Task NotifyUserAuthenticationAsync(string token)
    {
        // Parseia as claims do token JWT fornecido.
        var claims = ParseClaimsFromJwt(token);
        // Cria um ClaimsPrincipal para o usuário autenticado.
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        // Cria um Task<AuthenticationState> para o usuário autenticado.
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        // Notifica o Blazor sobre a mudança no estado de autenticação.
        NotifyAuthenticationStateChanged(authState);
        return Task.CompletedTask;
    }

    // Notifica a aplicação que o usuário fez logout.
    public Task NotifyUserLogoutAsync()
    {
        // Cria um ClaimsPrincipal para um usuário anônimo (não autenticado).
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        // Cria um Task<AuthenticationState> para o usuário anônimo.
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        // Notifica o Blazor sobre a mudança no estado de autenticação.
        NotifyAuthenticationStateChanged(authState);
        return Task.CompletedTask;
    }

    // Método privado para fazer o parse das claims de um token JWT.
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
} 