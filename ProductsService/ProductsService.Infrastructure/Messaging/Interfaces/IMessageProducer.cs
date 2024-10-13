namespace ProductsService.Infrastructure.Messaging.Interfaces
{
    public interface IMessageProducer
    {
        void Publish(string queueName, string message);
    }
}
