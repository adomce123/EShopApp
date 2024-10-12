namespace OrdersService.Infrastructure.Messaging.Settings
{
    public class RabbitMqSettings
    {
        public string? Hostname { get; set; }
        public string? OrderQueueName { get; set; }
        public string? ProductInfoQueueName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
