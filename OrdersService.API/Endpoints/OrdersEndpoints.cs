using OrdersService.Application.Orders.Queries;

namespace OrdersService.API.Endpoints
{
    public class OrdersEndpoints
    {
        private readonly GetOrdersQueryHandler _getOrdersQueryHandler;

        public OrdersEndpoints(GetOrdersQueryHandler getOrdersQueryHandler)
        {
            _getOrdersQueryHandler = getOrdersQueryHandler;
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", GetAllOrders);
        }

        private IResult GetAllOrders()
        {
            var query = new GetOrdersQuery();
            var result = _getOrdersQueryHandler.Handle(query);
            return Results.Ok(result);
        }
    }
}
