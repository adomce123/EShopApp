using ProductsService.Core.Services.Interfaces;
using ProductsService.Core.Services.Models;
using ProductsService.Core.Services.Models.Extensions;
using ProductsService.Infrastructure.Repositories.Interfaces;

namespace ProductsService.Core.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _repository;

        public ProductsService(IProductsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> GetAll()
        {
            var productEntities = await _repository.GetAll();

            return productEntities.Select(entity => entity.ToDto());
        }

        public async Task<ProductDto?> GetSingleById(int id)
        {
            var product = await _repository.GetSingleById(id);

            return product?.ToDto();
        }

        public async Task<ProductDto> Create(ProductDto productDto)
        {
            var createdProductEntity = await _repository.Create(productDto.ToEntity());

            return createdProductEntity.ToDto();
        }

        public async Task<bool> Update(int id, ProductDto productDto)
        {
            var entity = await _repository.GetSingleById(id);

            if (entity != null)
            {
                var entityToUpdate = entity.ToUpdateEntity(productDto);

                await _repository.Update(entityToUpdate);

                return true;
            }

            return false;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _repository.GetSingleById(id);

            if (entity != null)
            {
                await _repository.Delete(entity);

                return true;
            }

            return false;
        }
    }
}
