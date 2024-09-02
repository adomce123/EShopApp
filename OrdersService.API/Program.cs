using OrdersService.API.Endpoints;
using OrdersService.Application.Orders.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<GetOrdersQueryHandler>();

builder.Services.AddSingleton<OrdersEndpoints>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var ordersEndpoints = app.Services.GetRequiredService<OrdersEndpoints>();
ordersEndpoints.MapEndpoints(app);

app.Run();
