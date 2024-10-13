namespace OrdersService.Application.Messaging
{
    public interface IMessageProducer
    {
        void Publish(string queueName, string message);
    }
}
