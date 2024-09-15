using MediatR;
using OrdersService.Application.Orders.Interfaces;

namespace OrdersService.Application.Orders.Commands;
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        return await _orderRepository.CreateOrderAsync(command.CustomerId, command.TotalPrice, DateTime.UtcNow);
    }
}
