using EshopApiGateway.Middleware;
using EshopApiGateway.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var authSettingsSection = builder.Configuration.GetSection("AuthSettings");
services.Configure<AuthSettings>(authSettingsSection);

var authSettings = authSettingsSection.Get<AuthSettings>()
    ?? throw new ArgumentException("Auth settings is null");

// Add Authentication with Auth0 JWT Bearer
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(cfg =>
    {
        cfg.Authority = authSettings.Authority;
        cfg.Audience = authSettings.Audience;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authSettings.ValidIssuer,
            ValidateAudience = true,
            ValidAudiences = authSettings.ValidAudiences,
            ValidateLifetime = authSettings.ValidateLifetime,
            ValidateIssuerSigningKey = authSettings.ValidateIssuerSigningKey,
            ClockSkew = TimeSpan.FromSeconds(authSettings.ClockSkew)
        };
    });

var originsName = "AllowReactApp";
var origins = configuration.GetValue<string>("WebsiteUrl");

if (!string.IsNullOrEmpty(origins))
{
    services.AddCors(options =>
    {
        options.AddPolicy(name: originsName,
            policy =>
            {
                policy.WithOrigins(origins);
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
    });
}

services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors(originsName);

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy(); //.RequireAuthorization(); //Apply Authorization Globally for All Routes

app.Run();
