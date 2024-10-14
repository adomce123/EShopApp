using MassTransit;
using MassTransit.Mediator;
using Messaging.MassTransit.Events;
using Messaging.MassTransit.States;
using Microsoft.Extensions.Logging;
using OrdersService.Application.Orders.Commands;

namespace OrdersService.Application.Orders.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly IMediator _mediator;
    public State ProductRequested { get; private set; }
    public State OrderCompleted { get; private set; }
    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<ProductValidated> ProductValidated { get; private set; }

    public OrderStateMachine(ILogger<OrderStateMachine> logger, IMediator mediator)
    {
        _mediator = mediator;

        // Mapping the instance state to a property to track state transitions
        InstanceState(x => x.CurrentState);

        // Correlating events with the CorrelationId to manage saga lifecycle
        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ProductValidated, x => x.CorrelateById(context => context.Message.CorrelationId));

        // Defining the initial state when OrderCreated event occurs
        Initially(
            When(OrderCreated)
                .Then(context =>
                {
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.Created = DateTime.UtcNow;

                    logger.LogInformation("Order {OrderId} received, with product ids: {ProductIds}. Product check initiated.", 
                        context.Data.OrderId, string.Join(", ", context.Data.OrderDetails.Select(od => od.ProductId)));
                })
                .TransitionTo(ProductRequested)
                .PublishAsync(context =>
                {
                    logger.LogInformation($"Publishing ProductRequest for OrderId: {context.Instance.OrderId}");

                    // Publish a ProductRequest event to request product details
                    return context.Init<ProductRequest>(new
                    {
                        OrderId = context.Instance.OrderId,
                        OrderDetails = context.Data.OrderDetails
                    });
                })
                .Then(context =>
                {
                    logger.LogInformation($"ProductRequest for OrderId {context.Instance.OrderId} published.");
                })
        );

        During(ProductRequested,
            When(ProductValidated)
                .ThenAsync(async context =>
                {
                    logger.LogInformation($"Saga is in state: {context.Instance.CurrentState}");
                    logger.LogInformation("Order {OrderId} validated. Valid product ids: {Ids}",
                        context.Data.OrderId, string.Join(", ", context.Data.OrderDetails.Select(od => od.ProductId)));
                    logger.LogInformation($"Order {context.Instance.OrderId} will be sent for persistance.");

                    var insertOrderCommand = new InsertOrderCommand
                    {
                        OrderId = context.Instance.OrderId,
                        OrderDetails = context.Data.OrderDetails
                    };

                    await _mediator.Send(insertOrderCommand);
                })
                .TransitionTo(OrderCompleted)
        );

        // Set the saga to be marked as complete when finalized
        SetCompletedWhenFinalized();
    }
}
