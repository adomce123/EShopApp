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

                    logger.LogWithOrderAndCorrelationIds("Received OrderCreated event for", context.Data.OrderId, context.Data.CorrelationId);
                    logger.LogInformation("Products check initiated for order {OrderId}, with product ids: {ProductIds}",
                        context.Data.OrderId, string.Join(", ", context.Data.OrderDetails.Select(od => od.ProductId)));
                })
                .TransitionTo(ProductRequested) // what happens here ?
                .PublishAsync(context =>
                {
                    logger.LogWithOrderAndCorrelationIds("Publishing ProductRequested event for", context.Data.OrderId, context.Instance.CorrelationId);
                    return context.Init<ProductRequest>(new
                    {
                        context.Instance.CorrelationId,
                        context.Data.OrderId,
                        context.Data.OrderDetails
                    });
                })
        );

        During(ProductRequested,
            When(ProductValidated)
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
                .TransitionTo(OrderCompleted)
        );

        SetCompletedWhenFinalized();
    }
}
