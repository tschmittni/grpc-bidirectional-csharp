using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcClient
{
    public class TunnelClientWorker : BackgroundService
    {
        private readonly ILogger<TunnelClientWorker> _logger;

        public TunnelClientWorker(ILogger<TunnelClientWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions());

            _logger.LogInformation($"Worker running at: {DateTime.Now}");
            var client = new TunnelMessaging.TunnelMessagingClient(channel);

            // just wait 5 seconds to make sure the service started up
            Thread.Sleep(5000);

            using var sendData = client.SendData(new Metadata());
            Console.WriteLine($"Connected successfully to the server.");
            var responseTask = Task.Run(async () =>
            {
                while (await sendData.ResponseStream.MoveNext(stoppingToken))
                {
                    Console.WriteLine($"Received Response Message: {sendData.ResponseStream.Current.Message}");
                }
            });

            // wait 10 seconds before sending the first messaga for better debugging
            Thread.Sleep(10000);

            Console.WriteLine($"Client is now sending the message");
            await sendData.RequestStream.WriteAsync(new TunnelMessage
            { 
                Name = "worker_client", 
                Message = "my message" 
            });
            Console.WriteLine($"Client has successfully sent the message");

            Console.ReadLine();
            await sendData.RequestStream.CompleteAsync();
        }
    }
}
