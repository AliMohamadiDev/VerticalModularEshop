namespace Catalog.Data.Seed;

public class CatalogDataSeeder : IDataSeeder
{
    private readonly CatalogDbContext _dbContext;

    public CatalogDataSeeder(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAllAsync()
    {
        if (!await _dbContext.Products.AnyAsync())
        {
            await _dbContext.Products.AddRangeAsync(InitialData.Products);
            await _dbContext.SaveChangesAsync();
        }
    }
}