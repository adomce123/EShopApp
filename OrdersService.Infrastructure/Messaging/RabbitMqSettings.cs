namespace OrdersService.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public string? Hostname { get; set; }
        public string? QueueName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
