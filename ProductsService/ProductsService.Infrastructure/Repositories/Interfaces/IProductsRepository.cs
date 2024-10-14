using ProductsService.Infrastructure.Entities;

namespace ProductsService.Infrastructure.Repositories.Interfaces
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> GetSingleById(int id);
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> requestedProductIds);
        Task<Product> Create(Product entity);
        Task<Product> Update(Product entity);
        Task UpdateProductQuantitiesAsync(Dictionary<int, int> productQuantities);
        Task Delete(Product entity);
    }
}
