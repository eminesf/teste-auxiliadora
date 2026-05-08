namespace RentalPipeline.Application.Events;

public interface IEventPublisher
{
  Task PublishAsync<T>(T @event) where T : class;
}