using System.Text.Json;
using System.Text.Json.Serialization;
using Basket.Data.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Data.Repositories;

public class CachedBasketRepository : IBasketRepository
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = {new ShoppingCartConverter(), new ShoppingCartItemConverter()}
    };

    private readonly IBasketRepository _basketRepository;
    private readonly IDistributedCache _cache;

    public CachedBasketRepository(IBasketRepository basketRepository, IDistributedCache cache)
    {
        _basketRepository = basketRepository;
        _cache = cache;
    }

    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        if (!asNoTracking)
        {
            return await _basketRepository.GetBasket(userName, false, cancellationToken);
        }

        var cachedBasket = await _cache.GetStringAsync(userName, cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket))
        {
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket, _options)!;
        }

        var basket = await _basketRepository.GetBasket(userName, asNoTracking, cancellationToken);

        await _cache.SetStringAsync(userName, JsonSerializer.Serialize(basket, _options), cancellationToken);

        return basket;
    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToke = default)
    {
        await _basketRepository.CreateBasket(basket, cancellationToke);

        await _cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket, _options), cancellationToke);

        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        await _basketRepository.DeleteBasket(userName, cancellationToken);

        await _cache.RemoveAsync(userName, cancellationToken);

        return true;
    }

    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        var result = await _basketRepository.SaveChangesAsync(userName, cancellationToken);

        if (userName is not null)
        {
            await _cache.RemoveAsync(userName, cancellationToken);
        }

        return result;
    }
}