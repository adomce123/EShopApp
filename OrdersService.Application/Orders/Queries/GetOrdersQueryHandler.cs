using OrdersService.Application.Dtos;
using OrdersService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Dapper;

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
            var connection = _context.Database.GetDbConnection();

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            string sql = @"SELECT ""Id"", ""TotalPrice"" FROM ""Orders"";";

            // Use Dapper to execute the query
            var orders = await connection.QueryAsync<OrderDto>(sql);

            return orders;
        }
    }
}
