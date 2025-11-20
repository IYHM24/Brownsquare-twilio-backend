namespace Brownsquare_twilio_backend.Middlewares
{
    /// <summary>
    /// Mostrar logs de las peticiones HTTP en consola
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Log de la petición entrante
            _logger.LogInformation("Incoming Request: {method} {url}",
                context.Request.Method,
                context.Request.Path);
            await _next(context);
            // Log de la respuesta saliente
            _logger.LogInformation("Outgoing Response: {statusCode}",
                context.Response.StatusCode);
        }
    }
}
