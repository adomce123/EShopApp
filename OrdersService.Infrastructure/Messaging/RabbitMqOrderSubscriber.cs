using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OrdersService.Application.Messaging;

namespace OrdersService.Infrastructure.Messaging
{
    public class RabbitMqOrderSubscriber : IMessageSubscriber
    {
        private readonly RabbitMqSettings _rabbitMqSettings;

        public RabbitMqOrderSubscriber(IOptions<RabbitMqSettings> rabbitMqSettings)
        {
            _rabbitMqSettings = rabbitMqSettings.Value;
        }

        public void Subscribe()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSettings.Hostname,
                UserName = _rabbitMqSettings.Username,
                Password = _rabbitMqSettings.Password,
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _rabbitMqSettings.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Process the message asynchronously
                await HandleMessageAsync(message);

                // Acknowledge that the message was successfully processed
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Start consuming messages from the queue
            channel.BasicConsume(queue: _rabbitMqSettings.QueueName,
                                 autoAck: false, // Manually acknowledge messages
                                 consumer: consumer);
        }

        private async Task HandleMessageAsync(string message)
        {
            await Task.Run(() => Console.WriteLine($"Processing message: {message}"));
        }
    }
}
