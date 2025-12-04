using Brownsquare_twilio_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace Brownsquare_twilio_backend.Filters
{
    public class AuthTwilioFilter: IAuthorizationFilter
    {
        private readonly ILogger<AuthTwilioFilter> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _headerToAnalise = "x-twilio-signature";
        private readonly SignatureService _signatureService;

        public AuthTwilioFilter(ILogger<AuthTwilioFilter> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _signatureService = new SignatureService();
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            _logger.LogInformation("Validando twilio signature ...");

            var request = context.HttpContext.Request;

            // Primero intentamos obtener la llave desde la cabecera
            var hasHeader = request.Headers.TryGetValue(_headerToAnalise, out var headerKey);

            // Si no hay cabecera, intentamos extraer la llave desde el body
            if (!hasHeader)
            {
                // Permitir leer el body varias veces
                request.EnableBuffering();

                string body;
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }

                if (!string.IsNullOrWhiteSpace(body))
                {
                    string? keyFromBody = null;

                    // Si es JSON, parseamos propiedades buscando posibles nombres para la llave
                    if (!string.IsNullOrEmpty(request.ContentType) && request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            using var doc = JsonDocument.Parse(body);
                            if (doc.RootElement.ValueKind == JsonValueKind.Object)
                            {
                                foreach (var prop in doc.RootElement.EnumerateObject())
                                {
                                    var nameLower = prop.Name.Trim().ToLowerInvariant();
                                    if (nameLower == "signature" || nameLower == "key" || nameLower == _headerToAnalise.ToLowerInvariant() || nameLower == "x-twilio-signature")
                                    {
                                        keyFromBody = prop.Value.GetString();
                                        break;
                                    }
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignorar parse error y continuar intentando otras estrategias
                        }
                    }
                    else
                    {
                        // Intentar parsear como form-urlencoded
                        var parsed = QueryHelpers.ParseQuery(body);
                        if (parsed.TryGetValue("signature", out var v) || parsed.TryGetValue("Signature", out v))
                        {
                            keyFromBody = v.ToString();
                        }
                        else if (parsed.TryGetValue("key", out v) || parsed.TryGetValue("Key", out v))
                        {
                            keyFromBody = v.ToString();
                        }
                        else if (parsed.TryGetValue(_headerToAnalise, out v))
                        {
                            keyFromBody = v.ToString();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(keyFromBody))
                    {
                        // Inyectamos la llave en las cabeceras con el nombre esperado por el servicio
                        request.Headers[_headerToAnalise] = keyFromBody;
                        hasHeader = true;
                    }
                }
            }

            if (!hasHeader)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Sin cabecera ni llave en el body"
                });
                return;
            }

            // Obtener el token
            string authToken = _configuration["JWT:TwilioSignature:TwilioAuthToken"] ?? "";

            // establecer el context en el servicio de firma 
            //_signature_service.setContext(context);

            // Generar la firma (se respeta la llamada original a validarRequest)
            //bool validarFirma = await _signatureService.validarRequest(authToken, _headerToAnalise);
            bool validarFirma = request.Headers[_headerToAnalise] == authToken;

            if (!validarFirma)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Acceso denegado"
                });
                return;
            }
        }
    }

}
