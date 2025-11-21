using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Brownsquare_twilio_backend.Filters
{
    public class AuthTwilioFilter:IActionFilter
    {
        private readonly ILogger<AuthTwilioFilter> _logger;
        private readonly IConfiguration _configuration;
        private Stopwatch ? _stopwatch;

        public AuthTwilioFilter(ILogger<AuthTwilioFilter> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Validando twilio signature ...");
            
            // Verificar existencia de la signature
            if (context.HttpContext.Request.Headers.ContainsKey("x-twilio-signature"))
            {
                // Si existe el header
                Console.WriteLine("Header presente. Ejecutando lógica...");
            }
            else
            {
                Console.WriteLine("Header no encontrado. No se ejecuta lógica.");
            }

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch!.Stop();
            _logger.LogInformation($"⏱️ Tiempo de ejecución: {_stopwatch.ElapsedMilliseconds} ms");
        }
    }

}
