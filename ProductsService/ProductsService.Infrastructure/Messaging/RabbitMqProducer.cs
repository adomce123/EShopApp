using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductsService.Infrastructure.Messaging.Interfaces;
using ProductsService.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using System.Text;

namespace ProductsService.Infrastructure.Messaging
{
    public class RabbitMqProducer : RabbitMqBase, IMessageProducer
    {
        private readonly ILogger<RabbitMqProducer> _logger;

        public RabbitMqProducer(IOptions<RabbitMqSettings> rabbitMqSettings, ILogger<RabbitMqProducer> logger)
            : base(rabbitMqSettings)
        {
            _logger = logger;
        }

        public void Publish(string queueName, string message)
        {
            DeclareQueue(queueName);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: null,
                                  body: body);

            _logger.LogInformation("Published message to queue {QueueName}: {Message}", queueName, message);
        }
    }
}
