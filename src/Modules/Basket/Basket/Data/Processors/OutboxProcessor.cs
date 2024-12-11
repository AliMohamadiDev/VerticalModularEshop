using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.Data.Processors;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBus _bus;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceProvider serviceProvider, IBus bus, ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
                var outboxMessages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedOn == null)
                    .ToListAsync(stoppingToken);

                foreach (var message in outboxMessages)
                {
                    var eventType = Type.GetType(message.Type);
                    if (eventType == null)
                    {
                        _logger.LogWarning("Could not resolve type: {Type}.", message.Type);
                        continue;
                    }

                    var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);
                    if (eventMessage == null)
                    {
                        _logger.LogWarning("Could not deserialize message: {Content}.", message.Content);
                        continue;
                    }

                    await _bus.Publish(eventMessage, stoppingToken);

                    message.ProcessedOn = DateTime.UtcNow;

                    _logger.LogInformation("Successfully processed outbox message with Id: {Id}.", message.Id);
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Adjust the delay as needed
        }
    }
}