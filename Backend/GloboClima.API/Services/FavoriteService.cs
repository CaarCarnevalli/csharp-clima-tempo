// Importações necessárias para o serviço de favoritos
using GloboClima.API.DTOs;
using GloboClima.API.Interfaces;
using GloboClima.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Define o namespace do serviço
namespace GloboClima.API.Services;

// Exceção personalizada para favoritos já existentes
public class FavoriteAlreadyExistsException : Exception
{
    public FavoriteAlreadyExistsException(string message) : base(message) { }
}

// Define o serviço de favoritos
public class FavoriteService : IFavoriteService
{
    // Repositório de favoritos
    private readonly IFavoriteRepository _favoriteRepository;

    // Construtor do serviço
    public FavoriteService(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    // Método para criar um ID único para o favorito
    private string CreateFavoriteId(string userId, string cityName) => $"{userId}#{cityName}";

    // Método para adicionar um favorito
    public async Task<FavoriteDto?> AddFavoriteAsync(string userId, AddFavoriteRequestDto favoriteDto)
    {
        if (string.IsNullOrWhiteSpace(userId) || favoriteDto == null || string.IsNullOrWhiteSpace(favoriteDto.CityName))
        {
            throw new ArgumentException("User ID and City Name cannot be empty.");
        }

        var favoriteId = CreateFavoriteId(userId, favoriteDto.CityName);
        var existingFavorite = await _favoriteRepository.GetByIdAsync(favoriteId);
        if (existingFavorite != null)
        {
            throw new FavoriteAlreadyExistsException($"A cidade '{favoriteDto.CityName}' já está nos seus favoritos.");
        }

        var favorite = new Favorite
        {
            UserId = userId,
            CityName = favoriteDto.CityName,
            Country = favoriteDto.Country,
            DateAdded = DateTime.UtcNow
        };

        await _favoriteRepository.AddAsync(favorite);

        return new FavoriteDto
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            CityName = favorite.CityName,
            Country = favorite.Country
        };
    }

    // Método para obter os favoritos de um usuário
    public async Task<IEnumerable<FavoriteDto>> GetFavoritesByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty.");
        }

        var favorites = await _favoriteRepository.GetByUserIdAsync(userId);
        return favorites.Select(f => new FavoriteDto
        {
            Id = f.Id,
            UserId = f.UserId,
            CityName = f.CityName,
            Country = f.Country
        }).ToList();
    }

    // Método para remover um favorito
    public async Task<bool> RemoveFavoriteAsync(string userId, string favoriteIdParam)
    {
        Console.WriteLine($"FavoriteService: RemoveFavoriteAsync called. UserId from token: '{userId}', favoriteIdParam from controller: '{favoriteIdParam}'");
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(favoriteIdParam))
        {
            throw new ArgumentException("User ID and Favorite ID cannot be empty.");
        }

        var favorite = await _favoriteRepository.GetByIdAsync(favoriteIdParam);
        
        if (favorite == null)
        {
            Console.WriteLine($"FavoriteService: Favorite not found in repository for id: '{favoriteIdParam}'");
            return false;
        }
        
        Console.WriteLine($"FavoriteService: Favorite found. favorite.UserId: '{favorite.UserId}', userId from token: '{userId}'");

        if (favorite.UserId != userId)
        {
            Console.WriteLine($"FavoriteService: Mismatch! favorite.UserId ('{favorite.UserId}') != userId from token ('{userId}'). Denying deletion.");
            return false;
        }

        await _favoriteRepository.DeleteAsync(favoriteIdParam);
        Console.WriteLine($"FavoriteService: Successfully deleted favorite with id: '{favoriteIdParam}' from repository.");
        return true;
    }
}
