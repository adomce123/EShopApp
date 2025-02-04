﻿using Microsoft.EntityFrameworkCore;
using ProductsService.Infrastructure.Entities;
using ProductsService.Infrastructure.EntityFrameworkCore;
using ProductsService.Infrastructure.Repositories.Interfaces;

namespace ProductsService.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ProductsDbContext _context;

        public ProductsRepository(ProductsDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetSingleById(int id)
        {
            return await _context.Products
                .SingleOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> requestedProductIds)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => requestedProductIds.Contains(p.ProductId))
                .ToListAsync();
        }

        public async Task<Product> Create(Product entity)
        {
            _context.Products.Add(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<Product> Update(Product entity)
        {
            _context.Products.Update(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateProductQuantitiesAsync(Dictionary<int, int> productQuantities)
        {
            foreach (var productQuantity in productQuantities)
            {
                var product = await GetSingleById(productQuantity.Key);
                if (product != null)
                {
                    product.StockQuantity -= productQuantity.Value;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(Product entity)
        {
            _context.Products.Remove(entity);

            await _context.SaveChangesAsync();
        }
    }
}
