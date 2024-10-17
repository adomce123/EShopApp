using MediatR;
using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders.Commands
{
    public class SaveOrderCommand : IRequest<Unit>
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
    }
}
