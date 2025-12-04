using Grpc.Core;
using Grpc.Net.Client;
using Health;
using WhatsApp;

namespace Brownsquare_twilio_backend.Services
{
    public class WhatsAppGrpcClient: IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly Health.Health.HealthClient _healthClient;
        private readonly WhatsAppService.WhatsAppServiceClient _whatsappClient;
        private readonly string _serverAddress;

        public WhatsAppGrpcClient(string serverAddress = "http://localhost:50051")
        {
            _serverAddress = serverAddress;

            // Configurar el canal gRPC
            _channel = GrpcChannel.ForAddress(_serverAddress, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure, // ← AÑADIR ESTA LÍNEA para habilitar testeo en HTTP
                HttpHandler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                }
            });

            _healthClient = new Health.Health.HealthClient(_channel);
            _whatsappClient = new WhatsAppService.WhatsAppServiceClient(_channel);
        }

        /// <summary>
        /// Verifica el estado del servicio WhatsApp
        /// </summary>
        public async Task<HealthCheckResponse> CheckHealthAsync()
        {
            try
            {
                var request = new HealthCheckRequest { Service = "whatsapp" };
                return await _healthClient.CheckAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar health: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Envía un mensaje de WhatsApp
        /// </summary>
        public async Task<SendMessageResponse> SendMessageAsync(
            string numero,
            string codigoPais,
            string mensaje,
            string? messageId = null)
        {
            try
            {
                var request = new SendMessageRequest
                {
                    Numero = numero,
                    CodigoPais = codigoPais,
                    Mensaje = mensaje,
                    MessageId = messageId ?? string.Empty,
                    Type = MessageType.Text
                };

                return await _whatsappClient.SendMessageAsync(request);
            }
            catch (Grpc.Core.RpcException ex)
            {
                throw new Exception($"Error gRPC al enviar mensaje: {ex.Status.Detail}", ex);
            }
        }

        /// <summary>
        /// Obtiene el estado de un mensaje enviado
        /// </summary>
        public async Task<MessageStatusResponse> GetMessageStatusAsync(string messageId)
        {
            try
            {
                var request = new MessageStatusRequest
                {
                    MessageId = messageId
                };

                return await _whatsappClient.GetMessageStatusAsync(request);
            }
            catch (Grpc.Core.RpcException ex)
            {
                throw new Exception($"Error gRPC al obtener estado: {ex.Status.Detail}", ex);
            }
        }

        /// <summary>
        /// Obtiene el estado actual de la conexión con WhatsApp
        /// </summary>
        public async Task<ConnectionStatusResponse> GetConnectionStatusAsync()
        {
            try
            {
                var request = new ConnectionStatusRequest();
                return await _whatsappClient.GetConnectionStatusAsync(request);
            }
            catch (Grpc.Core.RpcException ex)
            {
                throw new Exception($"Error gRPC al obtener estado de conexión: {ex.Status.Detail}", ex);
            }
        }

        /// <summary>
        /// Suscribe a notificaciones en tiempo real del estado de conexión con WhatsApp
        /// </summary>
        /// <param name="onStatusReceived">Callback que se ejecuta cada vez que se recibe un nuevo estado</param>
        /// <param name="cancellationToken">Token para cancelar la suscripción</param>
        public async Task WatchConnectionStatusAsync(
            Action<ConnectionStatusResponse> onStatusReceived,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ConnectionStatusRequest { Watch = true };
                using var call = _whatsappClient.WatchConnectionStatus(request);

                await foreach (var status in call.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    onStatusReceived?.Invoke(status);
                }
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                // Ignorar cancelación normal
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en watch de conexión: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reinicia la conexión con WhatsApp
        /// </summary>
        /// <param name="force">Si es true, fuerza el reinicio aunque esté conectado</param>
        /// <param name="reason">Razón del reinicio (opcional)</param>
        public async Task<RestartConnectionResponse> RestartConnectionAsync(
            bool force = false,
            string? reason = null)
        {
            try
            {
                var request = new RestartConnectionRequest
                {
                    Force = force,
                    Reason = reason ?? string.Empty
                };

                return await _whatsappClient.RestartConnectionAsync(request);
            }
            catch (Grpc.Core.RpcException ex)
            {
                throw new Exception($"Error gRPC al reiniciar conexión: {ex.Status.Detail}", ex);
            }
        }

        /// <summary>
        /// Stream de monitoreo de salud (Health Watch)
        /// </summary>
        public async Task WatchHealthAsync(
            Action<HealthCheckResponse> onHealthReceived,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HealthCheckRequest { Service = "whatsapp" };
                using var call = _healthClient.Watch(request);

                await foreach (var health in call.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    onHealthReceived?.Invoke(health);
                }
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                // Ignorar cancelación normal
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en watch de health: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el estado de la conexión de forma segura (no lanza excepciones)
        /// </summary>
        public async Task<(bool IsConnected, string Message)> IsWhatsAppConnectedAsync()
        {
            try
            {
                var status = await GetConnectionStatusAsync();
                return (status.State == ConnectionState.Connected, status.Message);
            }
            catch
            {
                return (false, "Error al verificar conexión");
            }
        }


        /// <summary>
        /// Releases the resources used by the current instance of the class.
        /// </summary>
        /// <remarks>This method disposes of the underlying channel, if it is not null.  After calling
        /// this method, the instance should not be used further.</remarks>
        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
