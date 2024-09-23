using Microsoft.EntityFrameworkCore;
using OrdersService.API.Endpoints;
using OrdersService.Application.Messaging;
using OrdersService.Application.Orders;
using OrdersService.Application.Orders.Interfaces;
using OrdersService.Application.Orders.Queries;
using OrdersService.Infrastructure;
using OrdersService.Infrastructure.Data;
using OrdersService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("OrdersDBConnection") ??
    throw new ArgumentException("Connection string was not specified");

services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(connectionString));

services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetOrdersQueryHandler).Assembly));

services.AddSingleton<IMessageSubscriber, RabbitMqOrderSubscriber>();
services.AddSingleton<IOrderMessageHandler, OrderMessageHandler>();
services.AddSingleton<OrdersEndpoints>();
services.AddScoped<IOrderRepository, OrderRepository>();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var ordersEndpoints = app.Services.GetRequiredService<OrdersEndpoints>();
ordersEndpoints.MapEndpoints(app);


// Start consuming messages
var subscriber = app.Services.GetRequiredService<IMessageSubscriber>();
subscriber.Subscribe();  // This starts the RabbitMQ listener

await app.RunAsync();
