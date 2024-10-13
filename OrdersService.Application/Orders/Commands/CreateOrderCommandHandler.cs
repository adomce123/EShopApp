using MassTransit;
using MassTransitContracts;
using MediatR;

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

        var correlationId = Guid.NewGuid();

        // Publish the OrderCreated event to trigger the saga
        await _publishEndpoint.Publish(new OrderCreated
        {
            CorrelationId = correlationId,  // This is needed to track the saga
            OrderId = orderId,
            ProductIds = command.ProductIds
        }, cancellationToken);

        Console.WriteLine($"Order service published OrderCreated event with corr id : {correlationId}");

        // Return the Order ID immediately to the client (while the saga runs in the background)
        return orderId;
    }
}
