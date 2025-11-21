using Brownsquare_twilio_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsApp;

namespace Brownsquare_twilio_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("whatsapp")]
    public class WhatsAppController : ControllerBase
    {
        /// <summary>
        /// Variables
        /// </summary>
        private readonly ILogger<WhatsAppController> _logger;
        private readonly WhatsAppGrpcClient _whatsAppGrpcClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public WhatsAppController(ILogger<WhatsAppController> logger)
        {
            _logger = logger;
            _whatsAppGrpcClient = new WhatsAppGrpcClient();
        }

        /// <summary>
        /// Endpoint para obtener el estado de un mensaje enviado
        /// </summary>
        /// <param name="messageId">ID del mensaje a consultar</param>
        /// <returns>Estado del mensaje</returns>
        [HttpGet]
        [Route("message-status/{messageId}")]
        public async Task<IActionResult> GetMessageStatusAsync(string messageId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(messageId))
                {
                    return BadRequest("El ID del mensaje es requerido");
                }

                var response = await _whatsAppGrpcClient.GetMessageStatusAsync(messageId);

                if (response == null)
                {
                    return StatusCode(500, "Error al obtener el estado del mensaje");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener estado del mensaje {messageId}");
                _logger.LogError(ex.Message);
                return StatusCode(500, "Error interno al consultar el estado");
            }
        }

        /// <summary>
        /// Endpoint para obtener el estado actual de la conexión con WhatsApp
        /// </summary>
        /// <returns>Estado de la conexión</returns>
        [HttpGet]
        [Route("connection-status")]
        public async Task<IActionResult> GetConnectionStatusAsync()
        {
            try
            {
                var response = await _whatsAppGrpcClient.GetConnectionStatusAsync();

                if (response == null)
                {
                    return StatusCode(500, "Error al obtener el estado de conexión");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener estado de conexión de WhatsApp");
                _logger.LogError(ex.Message);
                return StatusCode(500, "Error interno al consultar el estado de conexión");
            }
        }

        /// <summary>
        /// Endpoint para verificar si WhatsApp está conectado (helper simplificado)
        /// </summary>
        /// <returns>True si está conectado, false en caso contrario</returns>
        [HttpGet]
        [Route("is-connected")]
        public async Task<IActionResult> IsConnectedAsync()
        {
            try
            {
                var isConnected = await _whatsAppGrpcClient.IsWhatsAppConnectedAsync();

                return Ok(new
                {
                    connected = isConnected,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al verificar conexión de WhatsApp");
                _logger.LogError(ex.Message);
                return StatusCode(500, "Error interno al verificar conexión");
            }
        }

        /// <summary>
        /// Endpoint para reiniciar la conexión con WhatsApp
        /// </summary>
        /// <param name="request">Parámetros del reinicio</param>
        /// <returns>Resultado del reinicio</returns>
        [HttpPost]
        [Route("restart-connection")]
        public async Task<IActionResult> RestartConnectionAsync([FromBody] RestartConnectionRequest? request)
        {
            try
            {
                var force = request?.Force ?? false;
                var reason = request?.Reason ?? "Reinicio manual desde API";

                var response = await _whatsAppGrpcClient.RestartConnectionAsync(force, reason);

                if (response == null)
                {
                    return StatusCode(500, "Error al reiniciar la conexión");
                }

                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al reiniciar conexión de WhatsApp");
                _logger.LogError(ex.Message);
                return StatusCode(500, "Error interno al reiniciar conexión");
            }
        }
    }
}
