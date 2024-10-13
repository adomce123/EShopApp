using ProductsService.Controllers.Models.Requests;
using ProductsService.Core.Services.Models;

namespace ProductsService.Controllers.Models.Extensions
{
    internal static class ProductsExtensions
    {
        internal static ProductDto ToDto(this ProductRequestDto request)
        {
            return new ProductDto
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            };
        }

        internal static ProductDto MapFromRequest(this ProductDto dtoToEdit, ProductRequestDto request)
        {
            dtoToEdit.Name = request.Name;
            dtoToEdit.Description = request.Description;
            dtoToEdit.Price = request.Price;
            dtoToEdit.StockQuantity = request.StockQuantity;

            return dtoToEdit;
        }
    }
}
