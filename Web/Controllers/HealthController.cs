using Brownsquare_twilio_backend.Services;
using Health;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brownsquare_twilio_backend.Controllers
{

    [Authorize]
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Variables
        /// </summary>
        private readonly ILogger<HealthController> _logger;
        private readonly WhatsAppGrpcClient _whatsAppGrpcClient;
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public HealthController(ILogger<HealthController> logger, IConfiguration config, WhatsAppGrpcClient whatsAppGrpcClient)
        {
            _logger = logger;
            _config = config;
            _whatsAppGrpcClient = whatsAppGrpcClient;
        }

        /// <summary>
        /// Endpoint para medir la salud del microservicio
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("check")]
        public async Task<HealthCheckResponse?> CheckHealthAsync()
        {
            try
            {
                HealthCheckResponse response_health = await _whatsAppGrpcClient.CheckHealthAsync();
                return response_health;
            }
            catch (Exception ex)
            {
                _logger.LogError("Fallo al obtener la salud del servicio WhatsApp");
                _logger.LogError("Direccion del servicio: " + _config["GrpcSettings:WhatsAppService:Address"]);
                _logger.LogError(ex.Message);
                BadRequest("Fallo al obtener la salud");
                return null;
            }
        }



    }
}
