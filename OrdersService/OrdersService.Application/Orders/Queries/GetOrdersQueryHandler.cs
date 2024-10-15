using MediatR;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Application.Orders.Dtos;

namespace OrdersService.Application.Orders.Queries
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetOrdersAsync();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId
            });
        }
    }
}
