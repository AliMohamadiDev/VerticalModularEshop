﻿using MassTransit;
using Ordering.Orders.Featured.CreateOrder;
using Shared.Messaging.Events;

namespace Ordering.Orders.EventHandlers;

public class BasketCheckoutIntegrationEventHandler
    : IConsumer<BasketCheckoutIntegrationEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<BasketCheckoutIntegrationEventHandler> _logger;

    public BasketCheckoutIntegrationEventHandler(ISender sender, ILogger<BasketCheckoutIntegrationEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
    {
        _logger.LogInformation("Integration event handled: {IntegrationEvent}.", context.Message.GetType().Name);

        var createOrderCommand = MapToCreateOrderCommand(context.Message);
        await _sender.Send(createOrderCommand);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutIntegrationEvent message)
    {
        var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine,
            message.Country, message.State, message.ZipCode);
        var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.Cvv,
            message.PaymentMethod);
        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto
        (
            Id: orderId,
            CustomerId: message.CustomerId,
            OrderName: message.UserName,
            ShippingAddress: addressDto,
            BillingAddress: addressDto,
            Payment: paymentDto,
            Items: new List<OrderItemDto>
            {
                new OrderItemDto(orderId, new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"), 2, 500),
                new OrderItemDto(orderId, new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"), 1, 400)

            }
        );

        return new CreateOrderCommand(orderDto);
    }
}