using Microsoft.Extensions.Logging;
using OrdersService.Application.Messaging;

namespace OrdersService.Application.Orders
{
    public class OrderMessageHandler : IOrderMessageHandler
    {
        private readonly ILogger<OrderMessageHandler> _logger;

        public OrderMessageHandler(ILogger<OrderMessageHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleMessageAsync(string message)
        {
            _logger.LogInformation("Processing order message: {Message}", message);
        }
    }
}
