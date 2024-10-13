namespace MassTransitContracts
{
    public class ProductRequest
    {
        public Guid CorrelationId { get; set; }
        public int OrderId { get; set; }
        public int[] ProductIds { get; set; }
    }
}
