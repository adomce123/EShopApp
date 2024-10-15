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
    private readonly MediatR.IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public State ProductRequestedState { get; private set; }
    public State OrderCompletedState { get; private set; }
    public Event<OrderCreated> OrderCreatedEvent { get; private set; }
    public Event<ProductValidated> ProductValidatedEvent { get; private set; }

    public OrderStateMachine(ILogger<OrderStateMachine> logger, MediatR.IMediator mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
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
                    context.Instance.OrderId = context.Data.OrderId;

                    logger.LogWithOrderAndCorrelationIds("Received OrderCreated event for", context.Data.OrderId, context.Data.CorrelationId);
                    logger.LogInformation("Products check initiated for order {OrderId}, with product ids: {ProductIds}",
                        context.Data.OrderId, string.Join(", ", context.Data.OrderDetails.Select(od => od.ProductId)));
                })
                .TransitionTo(ProductRequestedState)
                .PublishAsync(async context =>
                {
                    logger.LogWithOrderAndCorrelationIds("Publishing ProductRequested event for", context.Data.OrderId, context.Instance.CorrelationId);
                    return await context.Init<ProductRequest>(new
                    {
                        context.Instance.CorrelationId,
                        context.Data.OrderId,
                        context.Data.OrderDetails
                    });
                })
        );

        During(ProductRequestedState,
            When(ProductValidatedEvent)
                .ThenAsync(async context =>
                {
                    logger.LogWithOrderAndCorrelationIds("Received ProductValidated event for", context.Instance.OrderId, context.Instance.CorrelationId);
                    logger.LogInformation("Order {OrderId} validated. Valid product ids: {Ids}. Order persistance initiated.",
                        context.Data.OrderId, string.Join(", ", context.Data.OrderDetails.Select(od => od.ProductId)));

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var orderReadyForInsert = new InsertOrderCommand
                    {
                        OrderId = context.Instance.OrderId,
                        OrderDetails = context.Data.OrderDetails
                    };

                    await mediator.Send(orderReadyForInsert);
                })
                .TransitionTo(OrderCompletedState)
        );

        SetCompletedWhenFinalized();
    }
}
