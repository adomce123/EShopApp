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

        internal static Product ToUpdateEntity(this Product entity, ProductDto productDto)
        {
            entity.Name = productDto.Name;
            entity.Description = productDto.Description;
            entity.Price = productDto.Price;
            entity.StockQuantity = productDto.StockQuantity;

            return entity;
        }
    }
}
