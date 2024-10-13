using MassTransit;
using MassTransitContracts;
using OrdersService.Application.Orders.StateMachines.States;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State ProductRequested { get; private set; }
    public State OrderCompleted { get; private set; }

    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<ProductValidated> ProductValidated { get; private set; }

    public OrderStateMachine()
    {
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

                    Console.WriteLine($"Order {context.Data.OrderId} created. Product check initiated.");
                })
                .TransitionTo(ProductRequested)
                .PublishAsync(context =>
                {
                    Console.WriteLine($"Publishing ProductRequest for OrderId: {context.Instance.OrderId}");

                    // Publish a ProductRequest event to request product details
                    return context.Init<ProductRequest>(new
                    {
                        OrderId = context.Instance.OrderId,
                        ProductIds = context.Data.ProductIds
                    });
                })
                .Then(context =>
                {
                    Console.WriteLine($"ProductRequest for OrderId {context.Instance.OrderId} published.");
                })
        );

        During(ProductRequested,
            When(ProductValidated)
                .Then(context =>
                {
                    Console.WriteLine($"Saga is in state: {context.Instance.CurrentState}");
                    Console.WriteLine("ProductValidated event received.");
                    // Order processing is completed after product validation
                    context.Instance.Completed = DateTime.UtcNow;
                    Console.WriteLine($"Order {context.Instance.OrderId} completed after product validation.");
                })
                .TransitionTo(OrderCompleted)
        );

        // Set the saga to be marked as complete when finalized
        SetCompletedWhenFinalized();
    }
}
