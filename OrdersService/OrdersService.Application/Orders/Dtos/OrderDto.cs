namespace OrdersService.Application.Orders.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetails { get; set; } = Array.Empty<OrderDetailDto>();
    }
}
