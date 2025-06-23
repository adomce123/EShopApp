using OrdersService.Application.Orders.Commands;

namespace OrdersService.Application.Orders.Interfaces
{
    public interface IOrderValidator
    {
        string? ValidateOrderCommand(CreateOrderCommand command);
    }
}
