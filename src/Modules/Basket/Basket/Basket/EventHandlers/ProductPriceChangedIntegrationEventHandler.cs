using Basket.Basket.Features.UpdateItemPriceInBasket;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace Basket.Basket.EventHandlers;

public class ProductPriceChangedIntegrationEventHandler
    : IConsumer<ProductPriceChangedIntegrationEvent>
{
    private readonly ILogger<ProductPriceChangedIntegrationEventHandler> _logger;
    private readonly ISender _sender;

    public ProductPriceChangedIntegrationEventHandler(ILogger<ProductPriceChangedIntegrationEventHandler> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
    {
        _logger.LogInformation($"Integration Event Handled: {context.Message.GetType().Name}");

        var command = new UpdateItemPriceInBasketCommand(context.Message.ProductId, context.Message.Price);
        var result = await _sender.Send(command);
        
        if (!result.IsSuccess)
        {
            _logger.LogError($"Error updating price in basket for product id: {context.Message.ProductId}");
        }
        else
        {
            _logger.LogInformation($"Price for product id: {context.Message.ProductId} updated in basket");
        }
    }
}