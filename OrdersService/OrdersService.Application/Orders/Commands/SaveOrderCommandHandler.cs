using MediatR;
using Microsoft.Extensions.Logging;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders.Commands
{
    public class SaveOrderCommandHandler : IRequestHandler<SaveOrderCommand, Unit>
    {
        private readonly ILogger<SaveOrderCommandHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        public SaveOrderCommandHandler(ILogger<SaveOrderCommandHandler> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(SaveOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToInsert = new Order
            {
                Id = request.OrderId,
                OrderDate = DateTime.Now,
                TotalPrice = request.OrderDetails.Select(od => od.Price * od.Quantity).Sum(),
                OrderDetails = request.OrderDetails,
            };

            await _orderRepository.CreateOrderAsync(orderToInsert, cancellationToken);

            _logger.LogInformation("Order was inserted to database with order id: {Id}", request.OrderId);

            return Unit.Value;
        }
    }
}
