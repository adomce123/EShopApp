using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OrdersService.Application.Messaging;

namespace OrdersService.Infrastructure.Messaging
{
    public class RabbitMqOrderSubscriber : IMessageSubscriber, IDisposable
    {
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly IOrderMessageHandler _orderMessageHandler;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private bool _disposed;

        public RabbitMqOrderSubscriber(IOptions<RabbitMqSettings> rabbitMqSettings, IOrderMessageHandler orderMessageHandler)
        {
            _rabbitMqSettings = rabbitMqSettings.Value;
            _orderMessageHandler = orderMessageHandler;

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

            _channel.QueueDeclare(queue: _rabbitMqSettings.QueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void Subscribe()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await _orderMessageHandler.HandleMessageAsync(message);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: _rabbitMqSettings.QueueName,
                                  autoAck: false,
                                  consumer: consumer);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources (connection and channel)
                    _channel?.Close();
                    _connection?.Close();
                    _channel?.Dispose();
                    _connection?.Dispose();
                }

                // Set the flag to indicate the object has been disposed
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);  // Suppress finalization since we've already cleaned up
        }

        ~RabbitMqOrderSubscriber()
        {
            Dispose(disposing: false);  // Destructor calls Dispose with false
        }
    }
}
