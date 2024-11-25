using Shared.Pagination;

namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery(PaginationRequest PaginationRequest) : IQuery<GetProductsResult>;

public record GetProductsResult(PaginatedResult<ProductDto> Products);

public class GetProductsHandler : IQueryHandler<GetProductsQuery,GetProductsResult>
{
    private readonly CatalogDbContext _dbContext;

    public GetProductsHandler(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var totalCount = await _dbContext.Products.LongCountAsync(cancellationToken);

        var products = await _dbContext.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var productDtos = products.Adapt<List<ProductDto>>();

        return new GetProductsResult(
            new PaginatedResult<ProductDto>(pageIndex, pageSize, totalCount, productDtos));
    }
}