using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductsService.Infrastructure.Entities;

namespace ProductsService.Infrastructure.EntityFrameworkCore
{
    public class ProductsDbContext : IdentityDbContext<StoreUser>
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(7, 2)");


            builder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1000M, StockQuantity = 50 },
                new Product { ProductId = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 500M, StockQuantity = 100 }
            );
        }
    }
}
