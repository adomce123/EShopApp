using ProductsService.Core.Services.Models;

namespace ProductsService.Core.Services.Interfaces
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDto>> GetAll();
        Task<ProductDto?> GetSingleById(int id);
        Task<ProductDto> Create(ProductDto productDto);
        Task<bool> Update(int id, ProductDto productDto);
        Task<bool> Delete(int id);
    }
}
