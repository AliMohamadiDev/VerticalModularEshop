namespace Catalog.Contracts.Products.Features.GetProductById;

// We only move GetProductById to contracts because only this query will be expose to other modules
public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

public record GetProductByIdResult(ProductDto Product);