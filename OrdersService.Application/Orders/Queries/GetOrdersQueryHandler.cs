using OrdersService.Infrastructure;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using OrdersService.Application.Dtos;

namespace OrdersService.Application.Orders.Queries
{
    public class GetOrdersQueryHandler
    {
        private readonly OrdersDbContext _context;

        public GetOrdersQueryHandler(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> HandleAsync(GetOrdersQuery query)
        {
            var connection = _context.Database.GetDbConnection(); // Get NpgsqlConnection from DbContext

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            string sql = @"SELECT ""Id"", ""TotalPrice"" FROM ""Orders"";";

            var orders = await connection.QueryAsync<OrderDto>(sql); // Execute query using Dapper

            return orders;
        }
    }
}
