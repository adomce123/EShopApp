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
        var orderId = new Random().Next(1000, 9999);

        var orderCreated = new OrderCreated
        {
            OrderId = orderId,
            ProductIds = command.ProductIds
        };

        // Publish the OrderCreated event to trigger the saga
        await _publishEndpoint.Publish(orderCreated, cancellationToken);

        Console.WriteLine($"Order service published OrderCreated event with corr id : {orderCreated.CorrelationId}");

        // Return the Order ID immediately to the client (while the saga runs in the background)
        return orderId;
    }
}
