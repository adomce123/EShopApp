using MediatR;
using OrdersService.Application.Orders.Commands;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Application.Orders.Queries;

namespace OrdersService.API.Endpoints
{
    public class OrdersEndpoints
    {
        private readonly IOrderValidator _orderValidator;

        public OrdersEndpoints(IOrderValidator orderValidator)
        {
            _orderValidator = orderValidator;
        }

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
                var validationError = _orderValidator.ValidateOrderCommand(command);
                if (validationError != null)
                    return Results.BadRequest(validationError);

                var orderId = await mediator.Send(command, context.RequestAborted);
                return Results.Created($"/orders/{orderId}", new { OrderId = orderId });
            });
        }
    }
}
