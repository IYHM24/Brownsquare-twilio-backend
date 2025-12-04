using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Twilio.Security;
using Twilio.TwiML.Messaging;

namespace Brownsquare_twilio_backend.Services
{
    public class SignatureService
    {
        private AuthorizationFilterContext? _context;

        /// <summary>
        /// Constructor del servicio de validación de firmas
        /// </summary>
        public SignatureService() {}

        /// <summary>
        /// Setter del contexto de la petición HTTP
        /// </summary>
        /// <param name="context"></param>
        public void setContext(AuthorizationFilterContext context)
        {
            _context = context;
        }

        public bool validarRequest(string authToken, string headerToAnalise)
        {
            if (_context == null)
                return false;

            var request = _context.HttpContext.Request;

            // Header real a analizar
            string twilioSignature = "x-twilio-signature";

            // URL exacta como Twilio la envió
            string fullUrl = $"{request.Scheme}://{request.Host}{request.Path}";

            // Extraer cuerpo form-urlencoded (si aplica)
            
            //Declarar reader
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            
            //Generar diccionario de parámetros
            Dictionary<string, string> dict = new Dictionary<string, string> {
                {"HOla", "HOla"}
            };
            
            // Crear validador de Twilio
            var validator = new RequestValidator(authToken);

            // Validar firma
            bool isValid = validator.Validate(fullUrl, dict, twilioSignature);

            return isValid;
        }


    }

}