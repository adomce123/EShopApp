using OrdersService.Application.Orders.Commands;
using OrdersService.Application.Orders.Queries;

namespace OrdersService.API.Endpoints
{
    public class OrdersEndpoints
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async (GetOrdersQueryHandler handler) =>
            {
                var query = new GetOrdersQuery();
                var result = await handler.HandleAsync(query);
                return Results.Ok(result);
            });

            app.MapPost("/orders", async (CreateOrderCommand command, CreateOrderCommandHandler handler) =>
            {
                if (command == null)
                {
                    return Results.BadRequest("Invalid order data.");
                }

                var orderId = await handler.HandleAsync(command);

                return Results.Created($"/orders/{orderId}", new { OrderId = orderId });
            });
        }
    }
}
