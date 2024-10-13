namespace OrdersService.Application.Messaging
{
    public interface IOrderMessageHandler
    {
        Task HandleMessageAsync(string message);
    }
}
