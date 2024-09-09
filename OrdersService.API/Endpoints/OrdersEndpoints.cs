using OrdersService.Application.Orders.Queries;

namespace OrdersService.API.Endpoints
{
    public class OrdersEndpoints
    {
        private readonly IServiceProvider _serviceProvider;

        public OrdersEndpoints(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", GetAllOrders);
        }

        private async Task<IResult> GetAllOrders()
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<GetOrdersQueryHandler>();
            var query = new GetOrdersQuery();
            var result = await handler.HandleAsync(query);
            return Results.Ok(result);

        }
    }
}
