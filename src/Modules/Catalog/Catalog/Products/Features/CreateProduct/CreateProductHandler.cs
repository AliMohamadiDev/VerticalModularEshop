namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto Product)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Product.Category).NotEmpty().WithMessage("Category is required.");
        RuleFor(x => x.Product.ImageUrl).NotEmpty().WithMessage("ImageUrl is required.");
        RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}

public class CreateProductHandler
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    private readonly CatalogDbContext _dbContext;

    public CreateProductHandler(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = CreateNewProduct(command.Product);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }

    private Product CreateNewProduct(ProductDto productDto)
    {
        var product = Product.Create(
            Guid.NewGuid(),
            productDto.Name,
            productDto.Category,
            productDto.Description,
            productDto.ImageUrl,
            productDto.Price);

        return product;
    }
}