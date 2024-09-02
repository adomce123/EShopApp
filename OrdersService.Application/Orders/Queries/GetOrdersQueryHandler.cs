using OrdersService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersService.Application.Orders.Queries
{
    public class GetOrdersQueryHandler
    {
        private readonly List<OrderDto> _orders;

        public GetOrdersQueryHandler()
        {
            // Sample data directly in DTO format
            _orders = new List<OrderDto>
            {
                new OrderDto { Id = 1, TotalPrice = 20.0m },
                new OrderDto { Id = 2, TotalPrice = 15.0m }
            };
        }

        public IEnumerable<OrderDto> Handle(GetOrdersQuery query)
        {
            return _orders;
        }
    }
}
