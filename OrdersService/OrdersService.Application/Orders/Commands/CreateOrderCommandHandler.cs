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
            OrderId = orderId,
            OrderDetails = command.OrderDetails
        };

        // Publish the OrderCreated event to trigger the saga
        await _publishEndpoint.Publish(orderCreated, cancellationToken);

        Console.WriteLine($"Order service published OrderCreated event with order id : {orderId}");

        // Return the Order ID immediately to the client (while the saga runs in the background)
        return orderId;
    }
}
