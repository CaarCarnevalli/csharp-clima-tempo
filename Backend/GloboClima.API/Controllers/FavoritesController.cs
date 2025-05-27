// Importações necessárias para o controlador de favoritos
using GloboClima.API.DTOs;
using GloboClima.API.Interfaces;
using GloboClima.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

// Define o namespace do controlador
namespace GloboClima.API.Controllers;

// Define o controlador de favoritos
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    // Serviço de favoritos
    private readonly IFavoriteService _favoriteService;

    // Construtor do controlador
    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    // Método para obter o ID do usuário a partir do token
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User ID not found in token claims.");
        }
        return userId;
    }

// Método para obter os favoritos do usuário
[HttpGet]
public async Task<IActionResult> GetFavorites()
{
    try
    {
        var userId = GetUserId();
        var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);
        return Ok(favorites);
    }
    catch (InvalidOperationException ex)
    {
         Console.WriteLine($"Error retrieving user ID: {ex.Message}");
         return Unauthorized("Unable to identify user.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting favorites: {ex}");
        return StatusCode(500, "An unexpected error occurred while retrieving favorites.");
    }
}

    // Método para adicionar um favorito
    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequestDto favoriteDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetUserId();
            var newFavorite = await _favoriteService.AddFavoriteAsync(userId, favoriteDto);
            if (newFavorite == null)
            {
                return BadRequest("Could not add favorite. It may already exist or the city was not found.");
            }
            return CreatedAtAction(nameof(GetFavorites), new { /* id = newFavorite.Id */ }, newFavorite);
        }
        catch (FavoriteAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
             Console.WriteLine($"Error retrieving user ID: {ex.Message}");
             return Unauthorized("Unable to identify user.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding favorite: {ex}");
            return StatusCode(500, "An unexpected error occurred while adding the favorite.");
        }
    }

// Método para remover um favorito
[HttpDelete("{favoriteId}")]
public async Task<IActionResult> RemoveFavorite(string favoriteId)
{
    Console.WriteLine($"FavoritesController: Attempting to remove favorite. Raw favoriteId from route: '{favoriteId}'"); // LOG 1
     try
    {
        var userId = GetUserId();
        Console.WriteLine($"FavoritesController: UserId from token: '{userId}'"); // LOG 2

        var success = await _favoriteService.RemoveFavoriteAsync(userId, favoriteId);
        
        Console.WriteLine($"FavoritesController: FavoriteService.RemoveFavoriteAsync returned: {success}"); // LOG 3

        if (!success)
        {
            return NotFound("Favorite not found or could not be removed.");
        }
        return NoContent();
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
         Console.WriteLine($"Error retrieving user ID: {ex.Message}");
         return Unauthorized("Unable to identify user.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error removing favorite: {ex}");
        return StatusCode(500, "An unexpected error occurred while removing the favorite.");
    }
}
}
