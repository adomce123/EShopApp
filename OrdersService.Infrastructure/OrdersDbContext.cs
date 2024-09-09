using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Models;

namespace OrdersService.Infrastructure
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring the Order entity
            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            // Configuring the OrderDetail entity
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => od.Id);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            // Seeding test data
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, OrderDate = DateTime.UtcNow.AddDays(-2), CustomerId = 1, TotalPrice = 100.00m },
                new Order { Id = 2, OrderDate = DateTime.UtcNow.AddDays(-1), CustomerId = 2, TotalPrice = 150.00m }
            );

            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, Price = 25.00m },
                new OrderDetail { Id = 2, OrderId = 1, ProductId = 2, Quantity = 1, Price = 50.00m },
                new OrderDetail { Id = 3, OrderId = 2, ProductId = 3, Quantity = 3, Price = 50.00m }
            );
        }
    }
}
