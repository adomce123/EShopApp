using MassTransit;
using Messaging.MassTransit.States;
using Microsoft.EntityFrameworkCore;
using OrdersService.API.Endpoints;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Application.Orders.Queries;
using OrdersService.Application.Orders.StateMachines;
using OrdersService.Infrastructure;
using OrdersService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var apiGatewayUrl = configuration["ApiGateway:BaseUrl"] ?? throw new ArgumentException("ApiGatewayUrl not specified");
services.AddCors(options =>
{
    options.AddPolicy("ApiGatewayOnly", policy =>
    {
        policy.WithOrigins(apiGatewayUrl)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var connectionString = configuration.GetConnectionString("OrdersDBConnection") ??
    throw new ArgumentException("Connection string was not specified");

services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(connectionString));

services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetOrdersQueryHandler).Assembly));

services.AddMassTransit(x =>
{
    // Register the OrderStateMachine and its state in the saga
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .InMemoryRepository(); // Redis can be used

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        // Receive endpoint for the saga, listens for saga-related messages (like ProductValidated)
        cfg.ReceiveEndpoint("ordersQueue", e =>
        {
            e.ConfigureSaga<OrderState>(context);  // This will handle ProductValidated and other events related to the saga
        });
    });
});

services.AddSingleton<OrdersEndpoints>();
services.AddScoped<IOrderRepository, OrderRepository>();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("ApiGatewayOnly");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var ordersEndpoints = app.Services.GetRequiredService<OrdersEndpoints>();
ordersEndpoints.MapEndpoints(app);

await app.RunAsync();
