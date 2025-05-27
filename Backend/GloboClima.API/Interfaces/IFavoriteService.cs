// Importações necessárias para o serviço de favoritos
using GloboClima.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

// Define o namespace das interfaces
namespace GloboClima.API.Interfaces;

// Interface para o serviço de favoritos
public interface IFavoriteService
{
    // Método para adicionar um favorito
    Task<FavoriteDto?> AddFavoriteAsync(string userId, AddFavoriteRequestDto favoriteDto);

    // Método para obter os favoritos de um usuário
    Task<IEnumerable<FavoriteDto>> GetFavoritesByUserIdAsync(string userId);

    // Método para remover um favorito
    Task<bool> RemoveFavoriteAsync(string userId, string favoriteId);
}
