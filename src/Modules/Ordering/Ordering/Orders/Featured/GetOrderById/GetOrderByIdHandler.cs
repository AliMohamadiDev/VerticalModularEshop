namespace Ordering.Orders.Featured.GetOrderById;

public record GetOrderByIdQuery(Guid Id)
    : IQuery<GetOrderByIdResult>;

public record GetOrderByIdResult(OrderDto Order);

public class GetOrderByIdHandler
    : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResult>
{
    private readonly OrderingDbContext _dbContext;

    public GetOrderByIdHandler(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetOrderByIdResult> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(query.Id);
        }

        var orderDto = order.Adapt<OrderDto>();

        return new GetOrderByIdResult(orderDto);
    }
}