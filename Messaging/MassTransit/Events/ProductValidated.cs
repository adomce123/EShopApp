namespace Messaging.MassTransit.Events
{
    public class ProductValidated
    {
        public Guid CorrelationId { get; set; }
        public int OrderId { get; set; }
        public bool IsProductAvailable { get; set; }
    }
}
