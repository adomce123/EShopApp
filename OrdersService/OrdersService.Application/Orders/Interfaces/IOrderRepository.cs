using OrdersService.Domain.Models;

namespace OrdersService.Application.Orders.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task CreateOrderAsync(Order orderToCreate, CancellationToken cancellationToken);
    }
}
