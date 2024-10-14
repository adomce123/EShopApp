using OrdersService.Domain.Models;

namespace Messaging.MassTransit.Events
{
    public class OrderCreated
    {
        public Guid CorrelationId { get; set; }
        public int OrderId { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = Array.Empty<OrderDetail>();
    }
}
