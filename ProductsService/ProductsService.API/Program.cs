using Microsoft.EntityFrameworkCore;
using ProductsService.Extensions;
using ProductsService.Infrastructure.EntityFrameworkCore;
using ProductsService.Infrastructure.Repositories;
using ProductsService.Infrastructure.Repositories.Interfaces;
using ProductsService.Core.Services.Interfaces;
using ProductsService.Core.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("ProductsServiceDb") ??
    throw new ArgumentException("Connection string was not specified");

services.AddDbContext<ProductsDbContext>(
    opt => opt.UseSqlServer(connectionString));

services.AddMassTransit(x =>
{
    x.AddConsumer<ProductsValidationConsumer>(); // Register the consumer for ProductRequest

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        // ProductService listens to the product-service-requests queue for ProductRequest messages
        cfg.ReceiveEndpoint("product-service-requests", e =>
        {
            e.ConfigureConsumer<ProductsValidationConsumer>(context);
        });
    });
});

services.AddScoped<IProductsService, ProductsService.Core.Services.ProductsService>();
services.AddScoped<IProductsRepository, ProductsRepository>();

services.AddControllers();

services.AddEndpointsApiExplorer();

services.ConfigureSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
