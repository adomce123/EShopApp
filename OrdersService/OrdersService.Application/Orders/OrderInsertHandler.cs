using MediatR;
using OrdersService.Application.Orders.Commands;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders
{
    public class OrderInsertHandler : IRequestHandler<InsertOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderInsertHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(InsertOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToInsert = new Order
            {
                Id = request.OrderId,
                OrderDate = DateTime.Now,
                TotalPrice = request.OrderDetails.Select(od => od.Price).Sum(),
                OrderDetails = request.OrderDetails,
            };

            await _orderRepository.CreateOrderAsync(orderToInsert); // use canc token

            return Unit.Value; // Return Unit instead of an int
        }
    }
}
