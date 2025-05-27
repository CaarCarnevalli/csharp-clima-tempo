// Importações necessárias para o repositório de favoritos no DynamoDB
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using GloboClima.API.Domain;
using GloboClima.API.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

// Define o namespace do repositório
namespace GloboClima.API.Repositories;

// Define o repositório de favoritos no DynamoDB
public class DynamoDbFavoriteRepository : IFavoriteRepository
{
    // Cliente DynamoDB
    private readonly IAmazonDynamoDB _dynamoDbClient;

    // Nome da tabela no DynamoDB
    private readonly string _tableName;

    // Constantes para os atributos da tabela
    private const string AttrId = "Id";
    private const string AttrUserId = "UserId";
    private const string AttrCityName = "CityName";
    private const string AttrCountry = "Country";
    private const string AttrDateAdded = "DateAdded";
    private const string GsiUserId = "UserId-index";

    // Construtor do repositório
    public DynamoDbFavoriteRepository(IAmazonDynamoDB dynamoDbClient, IConfiguration configuration)
    {
        _dynamoDbClient = dynamoDbClient;
        _tableName = configuration["AWS:DynamoDB:FavoritesTable"] ?? "GloboClimaFavorites";
    }

    // Método para criar um ID único para o favorito
    private string CreateFavoriteId(string userId, string cityName) => $"{userId}#{cityName}";

    // Método para adicionar um favorito
    public async Task AddAsync(Favorite favorite)
    {
        if (favorite == null || string.IsNullOrEmpty(favorite.UserId) || string.IsNullOrEmpty(favorite.CityName))
        {
            throw new ArgumentNullException(nameof(favorite), "Favorite, UserId, and CityName cannot be null or empty.");
        }
        var favoriteId = CreateFavoriteId(favorite.UserId, favorite.CityName);

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { AttrId, new AttributeValue { S = favoriteId } },
                { AttrUserId, new AttributeValue { S = favorite.UserId } },
                { AttrCityName, new AttributeValue { S = favorite.CityName } },
                { AttrCountry, new AttributeValue { S = favorite.Country ?? string.Empty } },
                { AttrDateAdded, new AttributeValue { S = favorite.DateAdded.ToString("o", CultureInfo.InvariantCulture) } }
            },
            ConditionExpression = $"attribute_not_exists({AttrId})"
        };

        try
        {
            await _dynamoDbClient.PutItemAsync(request);
            favorite.Id = favoriteId;
        }
        catch (ConditionalCheckFailedException)
        {
            Console.WriteLine($"Attempted to add an existing favorite: {favoriteId}");
        }
    }

    // Método para obter os favoritos de um usuário
    public async Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId)
    {
        var request = new QueryRequest
        {
            TableName = _tableName,
            IndexName = GsiUserId,
            KeyConditionExpression = $"{AttrUserId} = :v_userId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":v_userId", new AttributeValue { S = userId } }
            }
        };

        var response = await _dynamoDbClient.QueryAsync(request);
        return response.Items.Select(item => MapToFavorite(item)).ToList();
    }

    // Método para obter um favorito pelo ID
    public async Task<Favorite?> GetByIdAsync(string favoriteId)
    {
        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { AttrId, new AttributeValue { S = favoriteId } }
            }
        };

        var response = await _dynamoDbClient.GetItemAsync(request);
        if (response.Item == null || !response.Item.Any())
        {
            return null;
        }
        return MapToFavorite(response.Item);
    }

    // Método para deletar um favorito pelo ID
    public async Task DeleteAsync(string favoriteId)
    {
         var request = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { AttrId, new AttributeValue { S = favoriteId } }
            }
        };
        await _dynamoDbClient.DeleteItemAsync(request);
    }

    // Método para mapear um item do DynamoDB para um objeto Favorite
    private Favorite MapToFavorite(Dictionary<string, AttributeValue> item)
    {
        return new Favorite
        {
            Id = item.TryGetValue(AttrId, out var idVal) ? idVal.S : string.Empty,
            UserId = item.TryGetValue(AttrUserId, out var userIdVal) ? userIdVal.S : string.Empty,
            CityName = item.TryGetValue(AttrCityName, out var cityNameVal) ? cityNameVal.S : string.Empty,
            Country = item.TryGetValue(AttrCountry, out var countryVal) ? countryVal.S : string.Empty,
            DateAdded = item.TryGetValue(AttrDateAdded, out var dateAddedVal) &&
                        DateTime.TryParseExact(dateAddedVal.S, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsedDate)
                        ? parsedDate
                        : DateTime.MinValue
        };
    }
}
