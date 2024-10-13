namespace ProductsService.Infrastructure.Messaging.Interfaces
{
    public interface IMessageSubscriber
    {
        void Subscribe(string queueName, Func<string, Task> messageHandler);
    }
}
