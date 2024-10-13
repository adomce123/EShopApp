using ProductsService.Infrastructure.Entities;

namespace ProductsService.Core.Services.Models.Extensions
{
    internal static class ProductsExtensions
    {
        internal static ProductDto ToDto(this Product entity)
        {
            return new ProductDto
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                StockQuantity = entity.StockQuantity
            };
        }

        internal static Product ToEntity(this ProductDto dto)
        {
            return new Product
            {
                ProductId = dto.ProductId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };
        }
    }
}
