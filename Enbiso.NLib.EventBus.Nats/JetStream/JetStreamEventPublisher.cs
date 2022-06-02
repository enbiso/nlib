using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enbiso.NLib.EventBus.Nats.JetStream
{
    public class JetStreamEventPublisher: IEventPublisher
    {
        private readonly IJetStreamConnection _jetStreamConnection;
        private readonly NatsOptions _options;
        private readonly ILogger _logger;

        public JetStreamEventPublisher(IOptions<NatsOptions> options, ILogger<JetStreamEventPublisher> logger, IJetStreamConnection jetStreamConnection)
        {
            _options = options.Value;
            _logger = logger;
            _jetStreamConnection = jetStreamConnection;
        }

        public Task Publish<TEvent>(TEvent @event, string exchange, string eventType, CancellationToken cancellationToken) where TEvent : IEvent
        {
            exchange ??= _options.PublishExchange ?? _options.Exchanges.FirstOrDefault();
            
            var eventPath = $"{exchange}.{eventType ?? @event.GetType().Name}";
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            
            var policy = NatsPolicyBuilder.BuildPublishPolicy(_options.PublishRetryCount, _logger);
            
            _jetStreamConnection.VerifyJetStream(exchange);
            var js = _jetStreamConnection.GetJetStream();
            
            policy.Execute(() =>
            {
                js.PublishAsync(eventPath, body);
            });
            
            return Task.CompletedTask;
        }
    }
}