using MassTransit;
using MediatR;
using Messaging.MassTransit.Events;
using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders.Commands;
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var random = new Random();
        int orderId = random.Next(0, 10000);

        var orderCreated = new OrderCreated
        {
            CorrelationId = Guid.NewGuid(),
            OrderId = orderId,
            OrderDetails = command.OrderDetails.Select(od => new OrderDetail
            {
                OrderId = orderId,
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                Price = od.Price
            })
        };

        // Publish the OrderCreated event to trigger the saga
        await _publishEndpoint.Publish(orderCreated, cancellationToken);

        return orderId;
    }
}
