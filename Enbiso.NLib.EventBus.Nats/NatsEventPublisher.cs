using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enbiso.NLib.EventBus.Nats;

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

    public Task Publish<TEvent>(TEvent @event, string exchange, string eventType, CancellationToken cancellationToken) where TEvent : IEvent
    {
        exchange ??= _options.PublishExchange ?? _options.Exchanges.FirstOrDefault();
            
        var eventPath = $"{exchange}.{eventType ?? @event.GetType().Name}";
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        
        var policy = NatsPolicyBuilder.BuildPublishPolicy(_options.PublishRetryCount, _logger);

        _connection.VerifyConnection();
        var conn = _connection.GetConnection();
        
        policy.Execute(() => {
            conn.Publish(eventPath, body);
            conn.Flush();
        });
            
        return Task.CompletedTask;
    }
}