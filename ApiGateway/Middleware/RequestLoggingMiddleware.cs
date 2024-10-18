namespace EshopApiGateway.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Handling request for {Path}", context.Request.Path);

            await _next(context);

            if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
            {
                _logger.LogWarning("Request for {Path} failed with status code {StatusCode}", context.Request.Path, context.Response.StatusCode);
            }
            else
            {
                _logger.LogInformation("Request for {Path} returned status code {StatusCode}", context.Request.Path, context.Response.StatusCode);
            }
        }
    }
}
