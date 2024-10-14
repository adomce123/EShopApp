using MassTransit;
using Messaging.MassTransit.Events;
using Microsoft.Extensions.Logging;
using ProductsService.Infrastructure.Repositories.Interfaces;
using OrdersService.Domain.Models;

namespace ProductsService.Core.Services
{
    public class ProductRequestConsumer : IConsumer<ProductRequest>
    {
        private readonly ILogger<ProductRequestConsumer> _logger;
        private readonly IProductsRepository _productsRepository;

        public ProductRequestConsumer(ILogger<ProductRequestConsumer> logger, IProductsRepository productsRepository)
        {
            _logger = logger;
            _productsRepository = productsRepository;
        }

        public async Task Consume(ConsumeContext<ProductRequest> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received ProductRequest for OrderId: {OrderId}, Corr Id: {CorrelationId}", message.OrderId, message.CorrelationId);

            var validOrderDetails = new List<OrderDetail>();

            try
            {
                validOrderDetails = await ValidateProductsAsync(message.OrderDetails);

                if (validOrderDetails.Any())
                {
                    var validProductQuantities = validOrderDetails.ToDictionary(od => od.ProductId, od => od.Quantity);

                    await _productsRepository.UpdateProductQuantitiesAsync(validProductQuantities);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process product request.");
            }

            await context.Publish(new ProductValidated
            {
                CorrelationId = message.CorrelationId,
                OrderId = message.OrderId,
                OrderDetails = validOrderDetails
            });

            _logger.LogInformation("Published ProductValidated for OrderId: {OrderId}, Corr Id: {CorrelationId}", message.OrderId, message.CorrelationId);
        }

        private async Task<List<OrderDetail>> ValidateProductsAsync(IEnumerable<OrderDetail> orderDetails)
        {
            var validOrderDetails = new List<OrderDetail>();

            var productIds = orderDetails.Select(od => od.ProductId).ToList();
            var foundProducts = await _productsRepository.GetByIdsAsync(productIds);

            foreach (var orderDetail in orderDetails)
            {
                var product = foundProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);

                if (product != null && product.StockQuantity >= orderDetail.Quantity)
                {
                    validOrderDetails.Add(orderDetail);
                }
            }

            return validOrderDetails;
        }
    }
}
