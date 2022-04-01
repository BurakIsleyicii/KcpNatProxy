using KcpNatProxy.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KcpNatProxy.ServerApplication
{
    public class Server
    {
        private readonly KnpServerOptions _options;
        private readonly ILoggerFactory _loggerFactory;

        public Server(ILoggerFactory loggerFactory)
        {

            _loggerFactory = loggerFactory;
        }

        public async Task Start(CancellationToken stoppingToken)
        {
           await Task.Factory.StartNew(() =>
            {
                KnpServerOptions options = new();
                KnpServerListenOptions knpServerListenOptions = new();

                knpServerListenOptions.Mtu = 1460;
                knpServerListenOptions.EndPoint = "0.0.0.0:9866";
                options.Listen = knpServerListenOptions;
                options.Credential = "1234";

                var server = new KnpServer(options, _loggerFactory.CreateLogger<KnpServer>());
                server.RunAsync(stoppingToken);
            }).ConfigureAwait(false);

            Thread.Sleep(1000);
            ServiceInjection serviceInjection = new ServiceInjection();
            List<KnpServiceDescription> services = new List<KnpServiceDescription>();
            KnpServiceDescription knpServiceDescription = new KnpServiceDescription();
            knpServiceDescription.WindowSize = 0;
            knpServiceDescription.QueueSize = 0;
            knpServiceDescription.UpdateInterval = 0;
            knpServiceDescription.ServiceType = KnpServiceType.Tcp;
            knpServiceDescription.Relay = KnpVirtualBusRelayType.Always;
            knpServiceDescription.Name = "sdafa";
            knpServiceDescription.Listen = "0.0.0.0:9855";
            services.Add(knpServiceDescription);
            serviceInjection.UdpTcpServiceStart(services, _loggerFactory.CreateLogger<KnpServer>());
            while (true)
            {
                Thread.Sleep(1000);
            }

        }
    }
}
