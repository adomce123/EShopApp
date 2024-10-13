using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<int> CreateOrderAsync(int customerId, decimal totalPrice, DateTime orderDate);
    }
}
