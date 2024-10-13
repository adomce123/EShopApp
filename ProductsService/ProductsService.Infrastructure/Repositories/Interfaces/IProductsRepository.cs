using ProductsService.Infrastructure.Entities;

namespace ProductsService.Infrastructure.Repositories.Interfaces
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetAll();

        Task<Product?> GetSingleById(int id);
        Task<Product> Create(Product entity);
        Task<Product> Update(Product entity);
        Task Delete(Product entity);
    }
}
