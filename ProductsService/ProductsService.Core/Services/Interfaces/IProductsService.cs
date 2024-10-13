using ProductsService.Core.Services.Models;

namespace ProductsService.Core.Services.Interfaces
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDto>> GetAll();
        Task<ProductDto?> GetSingleById(int id);
        Task<ProductDto> Create(ProductDto productDto);
        Task Update(ProductDto productDto);
        Task Delete(ProductDto productDto);
    }
}
