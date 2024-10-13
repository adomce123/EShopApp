using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersService.Application.Messaging;
using OrdersService.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using System.Text;

namespace OrdersService.Infrastructure.Messaging
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
