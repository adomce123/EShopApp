using MassTransit;
using Messaging.MassTransit.Events;
using Messaging.Logging;
using Microsoft.Extensions.Logging;
using ProductsService.Infrastructure.Repositories.Interfaces;
using OrdersService.Domain.Models;

namespace ProductsService.Core.Services
{
    public class ProductsValidationConsumer : IConsumer<ProductsValidationRequested>
    {
        private readonly ILogger<ProductsValidationConsumer> _logger;
        private readonly IProductsRepository _productsRepository;

        public ProductsValidationConsumer(ILogger<ProductsValidationConsumer> logger, IProductsRepository productsRepository)
        {
            _logger = logger;
            _productsRepository = productsRepository;
        }

        public async Task Consume(ConsumeContext<ProductsValidationRequested> context)
        {
            var message = context.Message;
            _logger.LogWithOrderAndCorrelationIds("Received ProductRequest message for", message.OrderId, message.CorrelationId);

            var validOrderDetails = new List<OrderDetail>();

            try
            {
                validOrderDetails = await ValidateProductsAsync(message.OrderDetails);

                if (validOrderDetails.Any())
                {
                    var validProductQuantities = validOrderDetails.ToDictionary(od => od.ProductId, od => od.Quantity);

                    await _productsRepository.UpdateProductQuantitiesAsync(validProductQuantities);

                    _logger.LogInformation("Updated product quantities for order {OrderId}, for product ids: {ProductIds}",
                        message.OrderId, string.Join(',', validOrderDetails.Select(od => od.ProductId)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process ProductRequest");
            }

            await context.Publish(new ProductsValidated
            {
                CorrelationId = message.CorrelationId,
                OrderId = message.OrderId,
                OrderDetails = validOrderDetails
            });

            _logger.LogWithOrderAndCorrelationIds("Published ProductValidated event for", message.OrderId, message.CorrelationId);
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
