using MassTransit;
using Shared.Messaging.Events;

namespace Catalog.Products.EventHandlers;

public class ProductPriceChangedEventHandler : INotificationHandler<ProductPriceChangedEvent>
{
    private readonly ILogger<ProductPriceChangedEventHandler> _logger;
    private readonly IBus _bus;

    public ProductPriceChangedEventHandler(ILogger<ProductPriceChangedEventHandler> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);

        var integrationEvent = new ProductPriceChangedIntegrationEvent
        {
            ProductId = notification.Product.Id,
            Name = notification.Product.Name,
            Category = notification.Product.Category,
            Description = notification.Product.Description,
            ImageUrl = notification.Product.ImageUrl,
            Price = notification.Product.Price // set updated product price
        };

        await _bus.Publish(integrationEvent, cancellationToken);
    }
}