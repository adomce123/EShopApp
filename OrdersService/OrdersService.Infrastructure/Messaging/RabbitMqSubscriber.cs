using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersService.Application.Messaging;
using OrdersService.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OrdersService.Infrastructure.Messaging
{
    public class RabbitMqSubscriber : RabbitMqBase, IMessageSubscriber
    {
        private readonly ILogger<RabbitMqSubscriber> _logger;

        public RabbitMqSubscriber(IOptions<RabbitMqSettings> rabbitMqSettings, ILogger<RabbitMqSubscriber> logger)
            : base(rabbitMqSettings)
        {
            _logger = logger;
        }

        public void Subscribe(string queueName, Func<string, Task> messageHandler)
        {
            DeclareQueue(queueName);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await messageHandler(message);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
                }
            };

            _channel.BasicConsume(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            _logger.LogInformation("Subscribed to queue {QueueName}", queueName);
        }
    }
}
