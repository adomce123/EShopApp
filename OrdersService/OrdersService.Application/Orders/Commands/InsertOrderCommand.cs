using MediatR;
using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders.Commands
{
    public class InsertOrderCommand : IRequest<Unit>
    {
        public int OrderId { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
    }
}
