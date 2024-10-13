using MediatR;

namespace OrdersService.Application.Orders.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public int[] ProductIds { get; set; } = Array.Empty<int>();
    }
}
