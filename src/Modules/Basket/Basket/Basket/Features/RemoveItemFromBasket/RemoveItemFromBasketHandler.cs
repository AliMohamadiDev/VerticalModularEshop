namespace Basket.Basket.Features.RemoveItemFromBasket;

public record RemoveItemFromBasketCommand(string UserName, Guid ProductId)
    : ICommand<RemoveItemFromBasketResult>;

public record RemoveItemFromBasketResult(Guid Id);

public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
    public RemoveItemFromBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.");
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");
    }
}

public class RemoveItemFromBasketHandler
    : ICommandHandler<RemoveItemFromBasketCommand, RemoveItemFromBasketResult>
{
    private readonly IBasketRepository _basketRepository;

    public RemoveItemFromBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await _basketRepository.GetBasket(command.UserName, false, cancellationToken);

        shoppingCart.RemoveItem(command.ProductId);

        await _basketRepository.SaveChangesAsync(command.UserName, cancellationToken);

        return new RemoveItemFromBasketResult(shoppingCart.Id);
    }
}