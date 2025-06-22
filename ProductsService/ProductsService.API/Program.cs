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

var sqlUser = Environment.GetEnvironmentVariable("SQL_USER");
var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD");

var connectionTemplate = builder.Configuration.GetConnectionString("ProductsServiceDb");
var connectionString = connectionTemplate
    .Replace("{UserId}", sqlUser)
    .Replace("{Password}", sqlPassword);

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

app.UseCors("ApiGatewayOnly");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();

