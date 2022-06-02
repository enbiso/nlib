using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using NATS.Client;
using Polly;
using Polly.Retry;

namespace Enbiso.NLib.EventBus.Nats;

public class NatsPolicyBuilder
{
    public static RetryPolicy BuildPublishPolicy(int retryCount, ILogger logger)
    {
        var policy = Policy.Handle<NATSTimeoutException>()
            .Or<SocketException>()
            .WaitAndRetry(retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, _) => { logger.LogWarning("{Error}",ex.ToString()); });
        return policy;
    }

    public static RetryPolicy BuildConnectPolicy(ILogger logger)
    {
        return Policy.Handle<NATSNoServersException>()
            .WaitAndRetryForever(
                _ => TimeSpan.FromSeconds(2),
                (ex, _) => logger.LogWarning("{Error}", ex.Message) );
    }
}