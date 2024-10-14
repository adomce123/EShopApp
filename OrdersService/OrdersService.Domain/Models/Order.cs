namespace OrdersService.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        // Navigation property to represent the relationship with OrderDetails
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = Enumerable.Empty<OrderDetail>();
    }
}
