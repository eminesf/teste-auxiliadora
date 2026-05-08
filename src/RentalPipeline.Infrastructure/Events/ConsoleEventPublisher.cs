using System.Text.Json;
using Microsoft.Extensions.Logging;
using RentalPipeline.Application.Events;

namespace RentalPipeline.Infrastructure.Events;

// Implementação de desenvolvimento — em produção seria substituída por
// RabbitMQ, AWS SNS, Azure Service Bus, etc.
public class ConsoleEventPublisher(ILogger<ConsoleEventPublisher> logger) : IEventPublisher
{
  public Task PublishAsync<T>(T @event) where T : class
  {
    var eventName = typeof(T).Name;
    var payload = JsonSerializer.Serialize(@event, new JsonSerializerOptions
    {
      WriteIndented = true
    });

    logger.LogInformation(
        "[EVENT PUBLISHED] {EventName} → {Payload}",
        eventName,
        payload);

    return Task.CompletedTask;
  }
}