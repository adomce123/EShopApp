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
    x.AddConsumer<ProductsValidationConsumer>();

    x.UsingAzureServiceBus((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration["AzureServiceBus:ConnectionString"];
        var receiveQueue = configuration["AzureServiceBus:ReceiveQueue"];

        cfg.Host(connectionString);

        cfg.ReceiveEndpoint(receiveQueue!, e =>
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

