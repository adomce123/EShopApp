using OrdersService.Application.Orders.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Models;

namespace OrdersService.Infrastructure.Data;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        // Get the connection from DbContext
        var connection = _context.Database.GetDbConnection();

        if (connection.State == System.Data.ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        string sql = @"SELECT ""Id"", ""TotalPrice"" FROM ""Orders"";";
        return await connection.QueryAsync<Order>(sql);  // Query via Dapper
    }

    public async Task<int> CreateOrderAsync(int customerId, decimal totalPrice, DateTime orderDate)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State == System.Data.ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        string sql = @"
            INSERT INTO ""Orders"" (""CustomerId"", ""TotalPrice"", ""OrderDate"")
            VALUES (@CustomerId, @TotalPrice, @OrderDate)
            RETURNING ""Id"";";

        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            CustomerId = customerId,
            TotalPrice = totalPrice,
            OrderDate = orderDate
        });
    }
}
