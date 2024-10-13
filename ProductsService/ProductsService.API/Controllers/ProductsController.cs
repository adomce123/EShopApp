using Microsoft.AspNetCore.Mvc;
using ProductsService.Controllers.Models.Extensions;
using ProductsService.Controllers.Models.Requests;
using ProductsService.Core.Services.Interfaces;
using ProductsService.Core.Services.Models;
using ProductsService.Infrastructure.Repositories.Interfaces;

namespace ProductsService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IProductsRepository _repository;

        public ProductsController(IProductsService productsService, IProductsRepository repository)
        {
            _productsService = productsService;
            _repository = repository;
        }

        /// <summary>
        /// Gets a single product by given id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetSingleProduct(int id)
        {
            return Ok(await _productsService.GetSingleById(id));
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            return Ok(await _repository.GetAll());
        }

        /// <summary>
        /// Creates a product by given product request
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductRequestDto request)
        {
            var createdProduct = await _productsService.Create(request.ToDto());

            return CreatedAtAction(
                "GetSingleProduct",
                new { id = createdProduct.ProductId },
                createdProduct);
        }

        /// <summary>
        /// Deletes product by given id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductDto>> DeleteProduct(int id)
        {
            var product = await _productsService.GetSingleById(id);

            if (product is null)
                return NotFound();

            await _productsService.Delete(product);

            return NoContent();
        }

        /// <summary>
        /// Updates product by given id and product request
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, ProductRequestDto request)
        {
            var product = await _productsService.GetSingleById(id);

            if (product is null)
                return NotFound();

            var productToEdit = product.MapFromRequest(request);

            await _productsService.Update(productToEdit);

            return NoContent();
        }
    }
}
