using MediatR;
using OrdersService.Application.Orders.Dtos;

namespace OrdersService.Application.Orders.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public int CustomerId { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetails { get; set; } = Array.Empty<OrderDetailDto>();
    }
}
