using Microsoft.Extensions.Logging;
using ProductsService.Core.Services.Interfaces;

namespace ProductsService.Core.Services
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
