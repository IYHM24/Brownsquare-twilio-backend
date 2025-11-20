namespace Brownsquare_twilio_backend.Middlewares
{
    public class ApiKeyMiddleware
    {
        /// <summary>
        /// Variables del middleware
        /// </summary>
        private readonly RequestDelegate _next;
        private const string APIKEY_HEADER = "X-API-KEY";

        /// <summary>
        /// Constructor del middleware
        /// </summary>
        /// <param name="next"></param>
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método Invoke del middleware
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IConfiguration config)
        {

            //Validar si la API Key está presente en los headers
            if (!context.Request.Headers.TryGetValue(APIKEY_HEADER, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key no encontrada");
                return;
            }

            //Obtener el valor de la API Key desde la configuración
            var apiKey = config.GetValue<string>("JWT:ApiKey:key");

            //Validar si la API Key es correcta
            if (!apiKey!.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key inválida");
                return;
            }

            //Continuar con la siguiente etapa del pipeline
            await _next(context);
        }
    }
}
