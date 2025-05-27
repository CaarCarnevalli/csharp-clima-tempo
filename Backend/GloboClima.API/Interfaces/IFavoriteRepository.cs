// Importações necessárias para o repositório de favoritos
using GloboClima.API.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

// Define o namespace das interfaces
namespace GloboClima.API.Interfaces;

// Interface para o repositório de favoritos
public interface IFavoriteRepository
{
    // Método para adicionar um favorito
    Task AddAsync(Favorite favorite);

    // Método para obter os favoritos de um usuário
    Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId);

    // Método para obter um favorito pelo ID
    Task<Favorite?> GetByIdAsync(string favoriteId);

    // Método para deletar um favorito pelo ID
    Task DeleteAsync(string favoriteId);
}
