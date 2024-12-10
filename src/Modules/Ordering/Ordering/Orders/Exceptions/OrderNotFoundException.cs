using Shared.Exceptions;

namespace Ordering.Orders.Exceptions;

public class OrderNotFoundException : NotFoundException
{
    public OrderNotFoundException(Guid orderId) : base("Order", orderId)
    {
    }
}