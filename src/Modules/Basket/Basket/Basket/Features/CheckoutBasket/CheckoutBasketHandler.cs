using System.Text.Json;
using MassTransit;
using Shared.Messaging.Events;

namespace Basket.Basket.Features.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout)
    : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckout).NotNull().WithMessage("BasketCheckout can't be null.");
        RuleFor(x => x.BasketCheckout.UserName).NotEmpty().WithMessage("UserName is required.");
    }
}

public class CheckoutBasketHandler
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    private readonly BasketDbContext _dbContext;

    public CheckoutBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        // Checkout basket using Outbox pattern

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var basket = await _dbContext.ShoppingCarts
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.UserName == command.BasketCheckout.UserName, cancellationToken);

            if (basket == null)
            {
                throw new BasketNotFoundException(command.BasketCheckout.UserName);
            }

            var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
            eventMessage.TotalPrice = basket.TotalPrice;

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(eventMessage),
                OccurredOn = DateTime.UtcNow
            };

            _dbContext.OutboxMessages.Add(outboxMessage);

            _dbContext.ShoppingCarts.Remove(basket);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CheckoutBasketResult(true);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            return new CheckoutBasketResult(false);
        }

        // Checkout basket without Outbox pattern
        //var basket = await _basketRepository.GetBasket(command.BasketCheckout.UserName, true, cancellationToken);

        //var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
        //eventMessage.TotalPrice = basket.TotalPrice;

        //await _bus.Publish(eventMessage, cancellationToken);

        //await _basketRepository.DeleteBasket(command.BasketCheckout.UserName, cancellationToken);

        //return new CheckoutBasketResult(true);
    }
}