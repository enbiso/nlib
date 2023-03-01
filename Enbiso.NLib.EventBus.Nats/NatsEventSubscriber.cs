using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Enbiso.NLib.EventBus.Nats;

public class NatsEventSubscriber: IEventSubscriber
{
    private readonly INatsConnection _connection;
    private readonly IEventProcessor _eventProcessor;
    private readonly NatsOptions _options;

    public NatsEventSubscriber(INatsConnection connection, IOptions<NatsOptions> options, IEventProcessor eventProcessor)
    {
        _connection = connection;
        _eventProcessor = eventProcessor;
        _options = options.Value;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    public Task Subscribe(CancellationToken token = default)
    {
        _connection.Connected += conn =>
        {
            foreach (var exchange in _options.Exchanges ?? Array.Empty<string>())
            {
                conn.SubscribeAsync($"{exchange}.>", _options.Client, async (sender, args) =>
                {
                    var eventName = GetEventName(args.Message.Subject, exchange);
                    await _eventProcessor.ProcessEvent(eventName, args.Message.Data);
                });
            }
        };
        _connection.VerifyConnection();
        return Task.CompletedTask;
    }
    
    private static string GetEventName(string subject, string exchange) =>
        subject.StartsWith(exchange)
            ? subject[(exchange.Length + 1)..]
            : subject;
}