using OrdersService.Application.Messaging;

namespace OrdersService.Application.Orders
{
    public class OrderMessageHandler : IOrderMessageHandler
    {
        public async Task HandleMessageAsync(string message)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"Processing order message: {message}");
            });
        }
    }
}
