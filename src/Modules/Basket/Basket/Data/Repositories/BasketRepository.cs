namespace Basket.Data.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly BasketDbContext _dbContext;

    public BasketRepository(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ShoppingCarts
            .Include(x => x.Items)
            .Where(x => x.UserName == userName);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var basket = await query.SingleOrDefaultAsync(cancellationToken);

        return basket ?? throw new BasketNotFoundException(userName);
    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToke = default)
    {
        _dbContext.ShoppingCarts.Add(basket);
        await _dbContext.SaveChangesAsync(cancellationToke);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        var basket = await GetBasket(userName, false, cancellationToken);

        _dbContext.ShoppingCarts.Remove(basket);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}