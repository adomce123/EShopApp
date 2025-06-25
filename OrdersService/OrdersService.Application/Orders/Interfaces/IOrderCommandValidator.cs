using OrdersService.Application.Orders.Commands;

namespace OrdersService.Application.Orders.Interfaces
{
    public interface IOrderCommandValidator
    {
        string? ValidateOrderCommand(CreateOrderCommand command);
    }
}
