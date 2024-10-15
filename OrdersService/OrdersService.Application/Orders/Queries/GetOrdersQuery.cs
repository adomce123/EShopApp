using MediatR;
using OrdersService.Application.Orders.Dtos;

namespace OrdersService.Application.Orders.Queries
{
    public class GetOrdersQuery : IRequest<IEnumerable<OrderDto>>
    {
    }
}
