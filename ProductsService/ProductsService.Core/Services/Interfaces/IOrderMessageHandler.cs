namespace ProductsService.Core.Services.Interfaces
{
    public interface IOrderMessageHandler
    {
        Task HandleMessageAsync(string message);
    }
}
