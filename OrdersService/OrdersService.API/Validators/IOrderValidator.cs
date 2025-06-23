using OrdersService.Application.Orders.Commands;

namespace OrdersService.API.Validators
{
    public interface IOrderValidator
    {
        string? ValidateOrderCommand(CreateOrderCommand command);
    }
}
