using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using OrdersService.Infrastructure.Messaging.Settings;

namespace OrdersService.Infrastructure.Messaging
{
    public abstract class RabbitMqBase
    {
        protected readonly RabbitMqSettings _rabbitMqSettings;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;

        protected RabbitMqBase(IOptions<RabbitMqSettings> rabbitMqSettings)
        {
            _rabbitMqSettings = rabbitMqSettings.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSettings.Hostname,
                UserName = _rabbitMqSettings.Username,
                Password = _rabbitMqSettings.Password,
                RequestedHeartbeat = TimeSpan.FromSeconds(30),
                AutomaticRecoveryEnabled = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected void DeclareQueue(string queueName)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
