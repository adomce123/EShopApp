using MediatR;
using OrdersService.Application.Dtos;

namespace OrdersService.Application.Orders.Queries
{
    public class GetOrdersQuery : IRequest<IEnumerable<OrderDto>>
    {
    }
}
