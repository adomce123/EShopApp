using MassTransit;

namespace OrdersService.Application.Orders.StateMachines.States
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public int OrderId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Completed { get; set; }

    }
}
