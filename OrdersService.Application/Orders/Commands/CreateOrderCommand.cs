namespace OrdersService.Application.Orders.Commands
{
    public class CreateOrderCommand
    {
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
