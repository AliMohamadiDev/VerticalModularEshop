namespace Ordering.Orders.Featured.GetOrders;

public record GetOrdersQuery(PaginationRequest PaginationRequest)
    : IQuery<GetOrdersResult>;

public record GetOrdersResult(PaginatedResult<OrderDto> Orders);

public class GetOrdersHandler
    : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    private readonly OrderingDbContext _dbContext;

    public GetOrdersHandler(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var totalCount = await _dbContext.Orders.LongCountAsync(cancellationToken);

        var orders = await _dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .OrderBy(p => p.OrderName)
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var orderDtos = orders.Adapt<List<OrderDto>>();

        return new GetOrdersResult(
            new PaginatedResult<OrderDto>(
                pageIndex, pageSize, totalCount, orderDtos));
    }
}