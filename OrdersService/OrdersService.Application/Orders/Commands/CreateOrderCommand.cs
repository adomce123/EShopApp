using MediatR;
using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = Array.Empty<OrderDetail>();
    }
}
