using OrdersService.Application.Orders.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Models;
using System.Data.Common;
using System.Data;
using Microsoft.Extensions.Logging;

namespace OrdersService.Infrastructure.Data;

public class OrderRepository : IOrderRepository
{
    private readonly DbConnection _connection;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(ILogger<OrderRepository> logger, OrdersDbContext context)
    {
        _connection = context.Database.GetDbConnection();
        _logger = logger;
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

    public async Task CreateOrderAsync(Order orderToCreate)
    {
        await EnsureConnectionOpenAsync();

        await using var transaction = await _connection.BeginTransactionAsync();

        try
        {
            string insertOrderSql = @"
                INSERT INTO ""Orders"" (""CustomerId"", ""TotalPrice"", ""OrderDate"")
                VALUES (@CustomerId, @TotalPrice, @OrderDate)
                RETURNING ""Id"";";

            var orderId = await _connection.ExecuteScalarAsync<int>(insertOrderSql, new
            {
                orderToCreate.CustomerId,
                orderToCreate.TotalPrice,
                orderToCreate.OrderDate
            }, transaction);

            // Insert OrderDetails
            string insertOrderDetailsSql = @"
                INSERT INTO ""OrderDetails"" (""OrderId"", ""ProductId"", ""Quantity"", ""Price"")
                VALUES (@OrderId, @ProductId, @Quantity, @Price);";

            foreach (var orderDetail in orderToCreate.OrderDetails)
            {
                await _connection.ExecuteAsync(insertOrderDetailsSql, new
                {
                    orderId,
                    orderDetail.ProductId,
                    orderDetail.Quantity,
                    orderDetail.Price
                }, transaction);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed and was rollbacked while inserting order with id: {Id}", orderToCreate.Id);
            throw;
        }
    }
}
