// Importações necessárias para o repositório de favoritos de placeholder
using GloboClima.API.Domain;
using GloboClima.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Define o namespace do repositório
namespace GloboClima.API.Repositories;

// Este é um repositório de placeholder para desenvolvimento/teste antes de configurar o DynamoDB.
// Ele usa um dicionário em memória.
public class PlaceholderFavoriteRepository : IFavoriteRepository
{
    // Dicionário em memória para armazenar os favoritos
    private readonly Dictionary<string, Favorite> _favorites = new();

    // Método para criar um ID único para o favorito
    private string CreateFavoriteId(string userId, string cityName) => $"{userId}#{cityName}";

    // Método para adicionar um favorito
    public Task AddAsync(Favorite favorite)
    {
        if (favorite == null || string.IsNullOrEmpty(favorite.UserId) || string.IsNullOrEmpty(favorite.CityName))
        {
            Console.WriteLine($"PlaceholderRepo: AddAsync - Favorite, UserId, or CityName is null/empty.");
            return Task.CompletedTask;
        }
        favorite.Id = CreateFavoriteId(favorite.UserId, favorite.CityName);

        if (_favorites.ContainsKey(favorite.Id))
        {
            Console.WriteLine($"PlaceholderRepo: Favorite {favorite.Id} already exists. Not adding again.");
            return Task.CompletedTask;
        }

        _favorites[favorite.Id] = favorite;
        Console.WriteLine($"PlaceholderRepo: Added favorite {favorite.Id} for user {favorite.UserId}");
        return Task.CompletedTask;
    }

    // Método para obter os favoritos de um usuário
    public Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId)
    {
        var userFavorites = _favorites.Values.Where(f => f.UserId == userId).ToList();
        Console.WriteLine($"PlaceholderRepo: Found {userFavorites.Count} favorites for user {userId}");
        return Task.FromResult<IEnumerable<Favorite>>(userFavorites);
    }

    // Método para obter um favorito pelo ID
    public Task<Favorite?> GetByIdAsync(string favoriteId)
    {
        _favorites.TryGetValue(favoriteId, out var favorite);
        Console.WriteLine($"PlaceholderRepo: GetById {favoriteId} found: {favorite != null}");
        return Task.FromResult(favorite);
    }

    // Método para deletar um favorito pelo ID
    public Task DeleteAsync(string favoriteId)
    {
        if (_favorites.Remove(favoriteId))
        {
            Console.WriteLine($"PlaceholderRepo: Removed favorite {favoriteId}");
        }
        else
        {
             Console.WriteLine($"PlaceholderRepo: Favorite {favoriteId} not found for removal.");
        }
        return Task.CompletedTask;
    }
}
