using MediatR;
using OrdersService.Infrastructure;
using OrdersService.Application.Dtos;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace OrdersService.Application.Orders.Queries
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly OrdersDbContext _context;

        public GetOrdersQueryHandler(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
            }

            string sql = @"SELECT ""Id"", ""TotalPrice"" FROM ""Orders"";";
            var orders = await connection.QueryAsync<OrderDto>(sql); // Execute query using Dapper
            return orders;
        }
    }
}
