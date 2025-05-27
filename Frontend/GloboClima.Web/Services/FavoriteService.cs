// Importações necessárias para o serviço de favoritos
using GloboClima.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using System.Text.Json;
using System;

// Define o namespace do serviço
namespace GloboClima.Web.Services;

// Define o serviço de favoritos
public class FavoriteService
{
    // Cliente HTTP
    private readonly HttpClient _httpClient;

    // Serviço de armazenamento local
    private readonly ILocalStorageService _localStorage;

    // Construtor do serviço
    public FavoriteService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    // Método para adicionar o cabeçalho de autorização
    private async Task AddAuthorizationHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    // Método para adicionar um favorito
    public async Task<FavoriteResponseDto> AddFavoriteAsync(AddFavoriteRequestDto favoriteDto)
    {
        await AddAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Favorites", favoriteDto);

        if (response.IsSuccessStatusCode)
        {
            var addedFavorite = await response.Content.ReadFromJsonAsync<FavoriteDto>();
            return new FavoriteResponseDto { Favorite = addedFavorite, IsSuccess = true, StatusCode = (int)response.StatusCode };
        }
        else
        {
            string errorMessage = "Ocorreu um erro desconhecido.";
            if (response.Content != null)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(errorContent))
                {
                    errorMessage = errorContent;
                }
            }
            return new FavoriteResponseDto { ErrorMessage = errorMessage, IsSuccess = false, StatusCode = (int)response.StatusCode };
        }
    }

    // Método para obter favoritos por ID do usuário
    public async Task<IEnumerable<FavoriteDto>?> GetFavoritesByUserIdAsync()
    {
        await AddAuthorizationHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<FavoriteDto>>("api/Favorites");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro ao buscar favoritos: {ex.Message}");
            if(ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("Não autorizado ao buscar favoritos.");
            }
            return null;
        }
    }

    // Método para remover um favorito
    public async Task<ApiResponse> RemoveFavoriteAsync(string favoriteId)
    {
        if (string.IsNullOrEmpty(favoriteId))
        {
            return new ApiResponse
            {
                IsSuccess = false,
                ErrorMessage = "ID do favorito não pode estar vazio."
            };
        }

        try
        {
            await AddAuthorizationHeader();
            // Codifica o favoriteId para garantir que caracteres como '#' sejam tratados corretamente na URL
            string encodedFavoriteId = Uri.EscapeDataString(favoriteId);
            var response = await _httpClient.DeleteAsync($"api/Favorites/{encodedFavoriteId}"); // Usar o ID codificado

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { IsSuccess = true };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = !string.IsNullOrWhiteSpace(errorContent) ?
                    errorContent : "Erro ao remover favorito. O item pode não existir ou já ter sido removido."
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao remover favorito: {ex}");
            return new ApiResponse
            {
                IsSuccess = false,
                ErrorMessage = "Erro ao tentar remover o favorito. Por favor, tente novamente."
            };
        }
    }
}
