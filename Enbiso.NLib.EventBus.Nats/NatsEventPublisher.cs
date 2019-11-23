using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using Polly;

namespace Enbiso.NLib.EventBus.Nats
{
    public class NatsEventPublisher : IEventPublisher
    {
        private readonly ILogger _logger;
        private readonly NatsOptions _options;
        private readonly INatsConnection _connection;

        public NatsEventPublisher(IOptions<NatsOptions> options, INatsConnection connection,
            ILogger<NatsEventPublisher> logger)
        {
            _connection = connection;
            _logger = logger;
            _options = options.Value;
        }

        public Task Publish<T>(T @event, string exchange = null, CancellationToken cancellationToken = default) where T: IEvent
        {
            if (!_connection.IsConnected && !_connection.TryConnect()) return Task.CompletedTask;
            
            var conn = _connection.GetConnection();
            var eventName = $"{exchange ?? _options.Exchanges.FirstOrDefault()}.{@event.GetType().Name}";
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);

            var policy = Policy.Handle<NATSTimeoutException>()
                .Or<SocketException>()
                .WaitAndRetry(_options.PublishRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => { _logger.LogWarning(ex.ToString()); });

            policy.Execute(() => {
                conn.Publish(eventName, body);
                conn.Flush();
            });
            
            return Task.CompletedTask;
        }
    }
}