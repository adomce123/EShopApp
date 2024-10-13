namespace OrdersService.Application.Messaging
{
    public interface IMessageSubscriber
    {
        void Subscribe(string queueName, Func<string, Task> messageHandler);
    }
}
