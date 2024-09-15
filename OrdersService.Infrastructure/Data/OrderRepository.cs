using OrdersService.Application.Orders.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Models;
using System.Data.Common;
using System.Data;

namespace OrdersService.Infrastructure.Data;

public class OrderRepository : IOrderRepository
{
    private readonly DbConnection _connection;

    public OrderRepository(OrdersDbContext context)
    {
        _connection = context.Database.GetDbConnection();
    }

    private async Task EnsureConnectionOpenAsync()
    {
        if (_connection.State == ConnectionState.Closed)
        {
            await _connection.OpenAsync();
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        await EnsureConnectionOpenAsync();

        string sql = @"SELECT ""Id"", ""TotalPrice"" FROM ""Orders"";";
        return await _connection.QueryAsync<Order>(sql);  // Query via Dapper
    }

    public async Task<int> CreateOrderAsync(int customerId, decimal totalPrice, DateTime orderDate)
    {
        await EnsureConnectionOpenAsync();

        string sql = @"
            INSERT INTO ""Orders"" (""CustomerId"", ""TotalPrice"", ""OrderDate"")
            VALUES (@CustomerId, @TotalPrice, @OrderDate)
            RETURNING ""Id"";";

        return await _connection.ExecuteScalarAsync<int>(sql, new
        {
            CustomerId = customerId,
            TotalPrice = totalPrice,
            OrderDate = orderDate
        });
    }
}
