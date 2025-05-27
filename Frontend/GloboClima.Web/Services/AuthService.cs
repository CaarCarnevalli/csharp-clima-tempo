// Importações necessárias para o serviço de autenticação
using GloboClima.Web.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System;
using GloboClima.Web.Auth; // Namespace para CustomAuthenticationStateProvider

// Define o namespace do serviço
namespace GloboClima.Web.Services;

// Define o serviço de autenticação
public class AuthService
{
    // Cliente HTTP
    private readonly HttpClient _httpClient;

    // Serviço de armazenamento local
    private readonly ILocalStorageService _localStorage;

    // Provedor de estado de autenticação
    private readonly AuthenticationStateProvider _authStateProvider;

    // Construtor do serviço
    public AuthService(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    // Método para login
    public async Task<ApiResponse> LoginAsync(LoginRequestDto loginRequest)
    {
        try
        {
            // Envia requisição de login
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);

            // Verifica se a resposta foi bem-sucedida
            if (response.IsSuccessStatusCode)
            {
                // Lê a resposta como LoginResponseDto
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Armazena o token de autenticação
                    await _localStorage.SetItemAsync("authToken", loginResponse.Token);

                    // Atualiza o estado de autenticação
                    if (_authStateProvider is CustomAuthenticationStateProvider authStateProvider)
                    {
                        await authStateProvider.NotifyUserAuthenticationAsync(loginResponse.Token);
                    }

                    // Retorna resposta de sucesso
                    return new ApiResponse { IsSuccess = true };
                }
            }

            // Lê o conteúdo da resposta como string
            var errorContent = await response.Content.ReadAsStringAsync();
            // Retorna resposta de erro
            return new ApiResponse
            {
                IsSuccess = false,
                ErrorMessage = !string.IsNullOrEmpty(errorContent) ?
                    errorContent : "Erro ao fazer login. Por favor, verifique suas credenciais."
            };
        }
        catch (Exception ex)
        {
            // Registra o erro
            Console.WriteLine($"Erro no login: {ex}");
            // Retorna resposta de erro
            return new ApiResponse
            {
                IsSuccess = false,
                ErrorMessage = "Erro ao conectar com o servidor. Por favor, tente novamente."
            };
        }
    }

    // Método para logout
    public async Task LogoutAsync()
    {
        // Remove o token de autenticação
        await _localStorage.RemoveItemAsync("authToken");
        // Notifica o provedor de estado de autenticação
        if (_authStateProvider is CustomAuthenticationStateProvider authStateProvider)
        {
            await authStateProvider.NotifyUserLogoutAsync();
        }
    }

    // Método para registro
    public async Task<bool> Register(RegisterRequestDto registerRequest)
    {
        // Envia requisição de registro
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerRequest);
        // Retorna se a resposta foi bem-sucedida
        return response.IsSuccessStatusCode;
    }
}
