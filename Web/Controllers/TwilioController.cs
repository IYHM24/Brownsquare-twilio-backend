using Brownsquare_twilio_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Brownsquare_twilio_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("webhook/[controller]")]
    public class TwilioController : ControllerBase
    {
        private ILogger<TwilioController>? _logger; 

        public TwilioController(ILogger<TwilioController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("estado/test")]
        public IActionResult Test()
        {
            return Ok(new ResponseTwilio
            {
                status = true,
                msj = "Twilio Webhook is working"
            });
        }

        [HttpPost]
        [Route("test")]
        public IActionResult TestTwilio()
        {
            return Ok(new ResponseTwilio
            {
                status = true,
                msj = "Twilio Webhook is working"
            });
        }

        /// <summary>
        /// Generar una orden recibida desde Twilio
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("save/order")]
        public IActionResult SaveOrder()
        {
            try
            {
                // Lógica para guardar la orden recibida desde Twilio
                _logger?.LogInformation("Orden recibida desde Twilio y salvada correctamente.");
                return Ok(new ResponseTwilio
                {
                    status = true,
                    msj = "Order saved successfully"
                });
            }
            catch (Exception ex)
            {
                
                // En caso de error, devolver un mensaje adecuado
                _logger?.LogError(ex, "Error al salvar la orden desde Twilio");
                return BadRequest(new ResponseTwilio
                {
                    status = false,
                    msj = $"Error al salvar la orden"
                });
            }
        }
    }
}
