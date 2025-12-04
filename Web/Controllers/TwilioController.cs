using Brownsquare_twilio_backend.Filters;
using Brownsquare_twilio_backend.Models;
using Brownsquare_twilio_backend.Services;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Utils;

namespace Brownsquare_twilio_backend.Controllers
{
    [ApiController]
    [Route("webhook/[controller]")]
    public class TwilioController : ControllerBase
    {
        private ILogger<TwilioController>? _logger; 
        private readonly WhatsAppGrpcClient _whatsappGrpcClient;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del controlador Twilio
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="whatsappGrpcClient"></param>
        public TwilioController(
            ILogger<TwilioController> logger,
            WhatsAppGrpcClient whatsappGrpcClient,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _whatsappGrpcClient = whatsappGrpcClient;
            _configuration = configuration;
        }

        /// <summary>
        /// Conocer estado del servicio
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("estado/test")]
        public IActionResult Test()
        {
            return Ok(new ResponseTwilio
            {
                status = true,
                msj = "Twilio Webhook is working"
            });
        }

        /// <summary>
        /// Validar el estado del servicio - Twilio
        /// </summary>
        /// <param name="testBody"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("test")]
        [ServiceFilter(typeof(AuthTwilioFilter))]
        public IActionResult TestTwilio(TestTwilioDto testBody)
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
        [ServiceFilter(typeof(AuthTwilioFilter))]
        public async Task<IActionResult> SaveOrder(OrderDto order)
        {
            try
            {
                //1. Obtener datos del pedido
                string informacion_cliente = order.client_information ?? "";
                string numero_telefono = order.PhoneNumber ?? "";
                string tipo_pedido = order.orderType ?? "";
                string precio_sugerido = MathUtil.PriceCalculator(tipo_pedido).ToString();
                string message_id = ToolUtil.GenerateUniqueId();

                //2. Generar la notificacion a whatsapp del nuevo pedido
                await _whatsappGrpcClient.SendMessageAsync(

                    // Número de teléfono destino (administrador)
                    _configuration["WhatsAppService:PhoneToNotify"] ?? "3506930989",

                    // Código de país
                    _configuration["WhatsAppService:CountryCode"] ?? "57",

                    //Cuerpo del mensaje
                    $"Nuevo pedido recibido:\n\n" +
                    $"Información del cliente: {informacion_cliente}\n\n" +
                    $"Tipo de pedido: {tipo_pedido}\n\n" +
                    $"Precio sugerido: {precio_sugerido}\n\n" +
                    $"Por favor, proceder con la gestión del pedido.",

                    // Id del mensaje
                    message_id
                );

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
