using OrdersService.Infrastructure;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace OrdersService.Application.Orders.Commands
{
    public class CreateOrderCommandHandler
    {
        private readonly OrdersDbContext _context;

        public CreateOrderCommandHandler(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<int> HandleAsync(CreateOrderCommand command)
        {
            var connection = _context.Database.GetDbConnection(); // Get NpgsqlConnection from DbContext

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            string sql = @"
                INSERT INTO ""Orders"" (""CustomerId"", ""TotalPrice"", ""OrderDate"")
                VALUES (@CustomerId, @TotalPrice, @OrderDate)
                RETURNING ""Id"";";

            var orderId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                CustomerId = command.CustomerId,
                TotalPrice = command.TotalPrice,
                OrderDate = DateTime.UtcNow
            });

            return orderId;
        }
    }
}
