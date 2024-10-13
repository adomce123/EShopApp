namespace Messaging.MassTransit.Events
{
    public class OrderCreated
    {
        public Guid CorrelationId { get; set; }
        public int OrderId { get; set; }
        public int[] ProductIds { get; set; }
    }
}
