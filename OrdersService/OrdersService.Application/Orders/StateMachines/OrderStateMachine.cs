using MassTransit;
using Messaging.MassTransit.Events;
using Messaging.MassTransit.States;
using Messaging.Logging;
using Microsoft.Extensions.Logging;
using OrdersService.Application.Orders.Commands;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace OrdersService.Application.Orders.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly IServiceProvider _serviceProvider;

    public State? ProductRequestedState { get; }
    public State? OrderCompletedState { get; }
    public Event<OrderCreated>? OrderCreatedEvent { get; }
    public Event<ProductValidated>? ProductValidatedEvent { get; }

    public OrderStateMachine(ILogger<OrderStateMachine> logger, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        // Mapping the instance state to a property to track state transitions
        InstanceState(x => x.CurrentState);

        // Correlating events with the CorrelationId to manage saga lifecycle
        Event(() => OrderCreatedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ProductValidatedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;

                    logger.LogWithOrderAndCorrelationIds("Received OrderCreated event for", context.Message.OrderId, context.Message.CorrelationId);
                    logger.LogInformation("Products check initiated for order {OrderId}, with product ids: {ProductIds}",
                        context.Message.OrderId, string.Join(", ", context.Message.OrderDetails.Select(od => od.ProductId)));
                })
                .TransitionTo(ProductRequestedState)
                .PublishAsync(async context =>
                {
                    logger.LogWithOrderAndCorrelationIds("Publishing ProductRequested event for", context.Message.OrderId, context.Saga.CorrelationId);
                    return await context.Init<ProductRequest>(new
                    {
                        context.Saga.CorrelationId,
                        context.Message.OrderId,
                        context.Message.OrderDetails
                    });
                })
        );

        During(ProductRequestedState,
            When(ProductValidatedEvent)
                .ThenAsync(async context =>
                {
                    logger.LogWithOrderAndCorrelationIds("Received ProductValidated event for", context.Saga.OrderId, context.Saga.CorrelationId);
                    logger.LogInformation("Order {OrderId} validated. Valid product ids: {Ids}. Order persistance initiated.",
                        context.Message.OrderId, string.Join(", ", context.Message.OrderDetails.Select(od => od.ProductId)));

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var orderReadyForInsert = new InsertOrderCommand
                    {
                        OrderId = context.Saga.OrderId,
                        OrderDetails = context.Message.OrderDetails
                    };

                    await mediator.Send(orderReadyForInsert);
                })
                .TransitionTo(OrderCompletedState)
        );

        SetCompletedWhenFinalized();
    }
}
