using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KcpNatProxy.MemoryPool;
using KcpNatProxy.Server;
using Microsoft.Extensions.Logging;

namespace KcpNatProxy
{
    public class ServiceInjection
    {
        private Dictionary<string, IKnpService>? _services;
        private Dictionary<string, KnpVirtualBusService>? _virtualBuses;
        public void UdpTcpServiceStart(List<KnpServiceDescription> services, ILogger<KnpServer> _logger)
        {
            var _bufferPool = new PinnedMemoryPool(1460);
            foreach (KnpServiceDescription service in services)
            {
                string errorMessage = "";
                if (service.ServiceType == KnpServiceType.Tcp)
                {
                    if (!service.ValidateTcpUdp(out errorMessage, out IPEndPoint? ipEndPoint))
                    {
                        throw new ArgumentException(errorMessage, nameof(ServiceInjection));
                    }

                    _services ??= new(StringComparer.Ordinal);
                    var parameters = new KnpTcpKcpParameters(service.WindowSize, service.QueueSize, service.UpdateInterval, service.NoDelay);
                    _services.Add(service.Name, new KnpTcpService(service.Name, ipEndPoint, parameters, _logger));
                }
                else if (service.ServiceType == KnpServiceType.Udp)
                {
                    if (!service.ValidateTcpUdp(out errorMessage, out IPEndPoint? ipEndPoint))
                    {
                        throw new ArgumentException(errorMessage, nameof(ServiceInjection));
                    }

                    _services ??= new(StringComparer.Ordinal);
                    _services.Add(service.Name, new KnpUdpService(service.Name, ipEndPoint, _bufferPool, _logger));
                }
                else if (service.ServiceType == KnpServiceType.VirtualBus)
                {
                    if (!service.ValidateVirtualBus(out errorMessage))
                    {
                        throw new ArgumentException(errorMessage, nameof(ServiceInjection));
                    }

                    _virtualBuses ??= new(StringComparer.Ordinal);
                    _virtualBuses.Add(service.Name, new KnpVirtualBusService(service.Name, service.Relay, _logger));
                }
                else
                {
                    throw new ArgumentException($"Listen endpoint is not a valid IP endpoint for service {service.Name}.", nameof(ServiceInjection));
                }
            }

            if (_services is not null)
            {
                foreach (KeyValuePair<string, IKnpService> serviceKv in _services)
                {
                    serviceKv.Value.Start();
                }
            }
        }

    }
}
