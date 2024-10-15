using MassTransit;
using Messaging.MassTransit.Events;
using Messaging.MassTransit.States;
using Microsoft.Extensions.Logging;
using OrdersService.Application.Orders.Commands;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace OrdersService.Application.Orders.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly MediatR.IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public State ProductRequested { get; private set; }
    public State OrderCompleted { get; private set; }
    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<ProductValidated> ProductValidated { get; private set; }

    public OrderStateMachine(ILogger<OrderStateMachine> logger, MediatR.IMediator mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;

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

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var orderReadyForInsert = new InsertOrderCommand
                    {
                        OrderId = context.Instance.OrderId,
                        OrderDetails = context.Data.OrderDetails
                    };

                    await mediator.Send(orderReadyForInsert);
                })
                .TransitionTo(OrderCompleted)
        );

        // Set the saga to be marked as complete when finalized
        SetCompletedWhenFinalized();
    }
}
