using MediatR;
using OrdersService.Infrastructure;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace OrdersService.Application.Orders.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly OrdersDbContext _context;

        public CreateOrderCommandHandler(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
            }

            string sql = @"
                INSERT INTO ""Orders"" (""CustomerId"", ""TotalPrice"", ""OrderDate"")
                VALUES (@CustomerId, @TotalPrice, @OrderDate)
                RETURNING ""Id"";";

            // Execute query using Dapper
            var orderId = await connection.ExecuteScalarAsync<int>(sql, new 
            {
                command.CustomerId,
                command.TotalPrice,
                OrderDate = DateTime.UtcNow
            });

            return orderId;
        }
    }
}
