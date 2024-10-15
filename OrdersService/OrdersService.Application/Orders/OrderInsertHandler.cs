using MediatR;
using Microsoft.Extensions.Logging;
using OrdersService.Application.Orders.Commands;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders
{
    public class OrderInsertHandler : IRequestHandler<InsertOrderCommand, Unit>
    {
        private readonly ILogger<OrderInsertHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrderInsertHandler(ILogger<OrderInsertHandler> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
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

            await _orderRepository.CreateOrderAsync(orderToInsert, cancellationToken);

            _logger.LogInformation("Order was inserted to database with order id: {Id}", request.OrderId);

            return Unit.Value; // Return Unit instead of an int
        }
    }
}
