using OrdersService.Application.Orders.Commands;

namespace OrdersService.API.Validators
{
    public class OrderValidator : IOrderValidator
    {
        public string? ValidateOrderCommand(CreateOrderCommand command)
        {
            if (command.CustomerId <= 0)
                return "CustomerId must be greater than 0.";

            if (command.OrderDetails == null || !command.OrderDetails.Any())
                return "Order must contain at least one item.";

            foreach (var detail in command.OrderDetails)
            {
                if (detail.ProductId <= 0)
                    return "Each OrderDetail must have a valid ProductId.";
                if (detail.Quantity <= 0)
                    return "Each OrderDetail must have a Quantity greater than 0.";
                if (detail.TotalPrice <= 0)
                    return "Each OrderDetail must have a TotalPrice greater than 0.";
            }

            return null;
        }
    }
}
