using MassTransit;

namespace Messaging.MassTransit.States
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public string CurrentState { get; set; } = string.Empty;
        public Guid CorrelationId { get; set; }
        public int OrderId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Completed { get; set; }
        public int Version { get; set; }
    }
}
