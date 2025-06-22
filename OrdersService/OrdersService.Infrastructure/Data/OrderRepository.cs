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

    private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken = default)
    {
        if (_connection.State == ConnectionState.Closed)
        {
            await _connection.OpenAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        await EnsureConnectionOpenAsync();

        string sql = @"SELECT ""Id"", ""TotalPrice"" FROM ""Orders"";";
        return await _connection.QueryAsync<Order>(sql);
    }

    public async Task CreateOrderAsync(Order orderToCreate, CancellationToken cancellationToken)
    {
        await EnsureConnectionOpenAsync(cancellationToken);

        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken);

        try
        {
            string insertOrderSql = @"
                INSERT INTO ""Orders"" (""Id"", ""CustomerId"", ""TotalPrice"", ""OrderDate"")
                VALUES (@Id, @CustomerId, @TotalPrice, @OrderDate)
                RETURNING ""Id"";";

            var orderId = await _connection.ExecuteScalarAsync<int>(insertOrderSql, new
            {
                orderToCreate.Id,
                orderToCreate.CustomerId,
                orderToCreate.TotalPrice,
                orderToCreate.OrderDate
            }, transaction);

            string insertOrderDetailsSql = @"
                INSERT INTO ""OrderDetails"" (""OrderId"", ""ProductId"", ""Quantity"", ""TotalPrice"")
                VALUES (@OrderId, @ProductId, @Quantity, @TotalPrice);";

            foreach (var orderDetail in orderToCreate.OrderDetails)
            {
                await _connection.ExecuteAsync(insertOrderDetailsSql, new
                {
                    orderId,
                    orderDetail.ProductId,
                    orderDetail.Quantity,
                    orderDetail.TotalPrice
                }, transaction);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Transaction failed and was rollbacked while inserting order with id: {Id}", orderToCreate.Id);
        }
    }
}
