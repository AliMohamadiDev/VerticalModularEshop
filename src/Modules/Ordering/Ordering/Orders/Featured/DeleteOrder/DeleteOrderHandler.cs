namespace Ordering.Orders.Featured.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId)
    : ICommand<DeleteOrderResult>;

public record DeleteOrderResult(bool IsSuccess);

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderName is required");
    }
}

public class DeleteOrderHandler
    : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{
    private readonly OrderingDbContext _dbContext;

    public DeleteOrderHandler(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .FindAsync(command.OrderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(command.OrderId);
        }

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteOrderResult(true);
    }
}