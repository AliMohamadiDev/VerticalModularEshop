using Basket.Basket.Exceptions;

namespace Basket.Basket.Features.DeleteBasket;

public record DeleteBasketCommand(string UserName)
    : ICommand<DeleteBasketResult>;

public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketHandler
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    private readonly BasketDbContext _dbContext;

    public DeleteBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await _dbContext.ShoppingCarts
            .SingleOrDefaultAsync(x => x.UserName == command.UserName, cancellationToken);

        if (basket == null)
        {
            throw new BasketNotFoundException(command.UserName);
        }

        _dbContext.ShoppingCarts.Remove(basket);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteBasketResult(true);
    }
}