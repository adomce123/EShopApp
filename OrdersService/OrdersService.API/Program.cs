using MassTransit;
using Messaging.MassTransit.States;
using Microsoft.EntityFrameworkCore;
using OrdersService.API.Endpoints;
using OrdersService.API.Validators;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Application.Orders.Queries;
using OrdersService.Application.Orders.StateMachines;
using OrdersService.Infrastructure;
using OrdersService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var apiGatewayUrl = configuration["ApiGateway:BaseUrl"] ?? throw new ArgumentException("ApiGatewayUrl not specified");
var redisHostname = configuration["Redis:Hostname"] ?? throw new ArgumentException("Redis hostname not specified");

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
        .RedisRepository(redisHostname); // Use Redis for state persistance

    x.UsingAzureServiceBus((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration["AzureServiceBus:ConnectionString"];
        var receiveQueue = configuration["AzureServiceBus:ReceiveQueue"];

        cfg.Host(connectionString);

        cfg.ReceiveEndpoint(receiveQueue!, e =>
        {
            e.ConfigureSaga<OrderState>(context);
        });
    });
});

services.AddSingleton<OrdersEndpoints>();
services.AddSingleton<IOrderCommandValidator, OrderCommandValidator>();
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
