using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcService.Services
{
    public class TunnelService : TunnelMessaging.TunnelMessagingBase
    {
        private readonly ILogger<TunnelService> _logger;

        public TunnelService(ILogger<TunnelService> logger)
        {
            _logger = logger;
        }

        public override async Task SendData(IAsyncStreamReader<TunnelMessage> requestStream, IServerStreamWriter<TunnelMessage> responseStream, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            _logger.LogInformation($"Client connected to server: {httpContext.Connection.Id}");

            while (await requestStream.MoveNext())
            {
                _logger.LogInformation($"Received message from Client: {requestStream.Current.Message}");
                await responseStream.WriteAsync(new TunnelMessage
                {
                    Name = "Response",
                    Message = "Response Test Message"
                });
            }
            _logger.LogInformation($"Client disconnected");
        }

        public void Dispose()
        {
            _logger.LogInformation("Cleaning up");
        }
    }
}
