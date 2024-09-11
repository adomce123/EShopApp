using MediatR;
using OrdersService.Application.Orders.Commands;
using OrdersService.Application.Orders.Queries;

namespace OrdersService.API.Endpoints
{
    public class OrdersEndpoints
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async (IMediator mediator, HttpContext context) =>
            {
                var query = new GetOrdersQuery();
                var result = await mediator.Send(query, context.RequestAborted);
                return Results.Ok(result);
            });

            app.MapPost("/orders", async (CreateOrderCommand command, IMediator mediator, HttpContext context) =>
            {
                if (command == null)
                {
                    return Results.BadRequest("Invalid order data.");
                }

                var orderId = await mediator.Send(command, context.RequestAborted);
                return Results.Created($"/orders/{orderId}", new { OrderId = orderId });
            });
        }
    }
}
