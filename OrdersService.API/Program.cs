using Microsoft.EntityFrameworkCore;
using OrdersService.API.Endpoints;
using OrdersService.Application.Orders.Queries;
using OrdersService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("OrdersDBConnection") ??
    throw new ArgumentException("Connection string was not specified");

services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(connectionString));

services.AddScoped<GetOrdersQueryHandler>();

services.AddSingleton<OrdersEndpoints>();

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

app.Run();
