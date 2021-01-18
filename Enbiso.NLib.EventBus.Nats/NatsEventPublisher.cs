using System;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using Polly;

namespace Enbiso.NLib.EventBus.Nats
{
    public class NatsEventPublisher: IEventPublisher
    {
        private readonly INatsConnection _connection;
        private readonly NatsOptions _options;
        private readonly ILogger _logger;

        public NatsEventPublisher(INatsConnection connection, IOptions<NatsOptions> options, ILogger<NatsEventPublisher> logger)
        {
            _connection = connection;
            _options = options.Value;
            _logger = logger;
        }

        public Task Publish<TEvent>(TEvent @event, string exchange, CancellationToken cancellationToken) where TEvent : IEvent
        {
            exchange ??= _options.PublishExchange ?? _options.Exchanges.FirstOrDefault();
            
            var eventName = $"{exchange}.{@event.GetType().Name}";
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            var policy = Policy.Handle<NATSTimeoutException>()
                .Or<SocketException>()
                .WaitAndRetry(_options.PublishRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => { _logger.LogWarning(ex.ToString()); });

            _connection.VerifyConnection();
            
            var conn = _connection.GetConnection();
            policy.Execute(() => {
                conn.Publish(eventName, body);
                conn.Flush();
            });
            
            return Task.CompletedTask;
        }
    }
}