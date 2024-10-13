using Microsoft.OpenApi.Models;
using ProductsService.Filters;
using System.Reflection;

namespace ProductsService.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // Note: It must be lowercase.
                    BearerFormat = "JWT"
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }
    }
}
