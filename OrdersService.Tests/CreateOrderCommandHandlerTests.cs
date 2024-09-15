using Moq;
using OrdersService.Application.Orders.Commands;
using OrdersService.Application.Orders.Interfaces;

namespace OrdersService.Tests;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnOrderId()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = 123,
            TotalPrice = 50.00m
        };

        _orderRepositoryMock
            .Setup(repo => repo.CreateOrderAsync(
                It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<DateTime>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);

        _orderRepositoryMock
            .Verify(repo => repo.CreateOrderAsync(
                    123, 50.00m, It.IsAny<DateTime>()), Times.Once);
    }
}
