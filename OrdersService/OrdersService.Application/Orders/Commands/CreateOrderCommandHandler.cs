using MassTransit;
using MediatR;
using Messaging.MassTransit.Events;

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
            OrderDetails = command.OrderDetails
        };

        // Publish the OrderCreated event to trigger the saga
        await _publishEndpoint.Publish(orderCreated, cancellationToken);

        return orderId;
    }
}
