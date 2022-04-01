using KcpNatProxy.ServerApplication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace).AddConsole());

Server server = new Server(loggerFactory);
await server.Start(CancellationToken.None);
