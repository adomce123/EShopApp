using MassTransit;
using Messaging.MassTransit.Events;

namespace ProductsService.Core.Services
{
    public class ProductRequestConsumer : IConsumer<ProductRequest>
    {
        public async Task Consume(ConsumeContext<ProductRequest> context)
        {
            var message = context.Message;

            Console.WriteLine($"Received product request for OrderId: {message.OrderId}, Corr Id: {message.CorrelationId}");

            // Publish ProductValidated event back to the saga
            await context.Publish(new ProductValidated
            {
                CorrelationId = message.CorrelationId,
                OrderId = message.OrderId,
                IsProductAvailable = true
            });

            Console.WriteLine($"Published ProductValidated for OrderId: {message.OrderId}, Corr Id: {message.CorrelationId}");
        }
    }
}
