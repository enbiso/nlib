using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;

namespace Enbiso.NLib.EventBus.Nats
{
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
            _eventProcessor.Setup();
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
                    if (_options.JetStreamEnable)
                    {
                        SubscribeJetStream(exchange, token);
                    }
                    else
                    {
                        SubscribeNative(conn, exchange, token);
                    }
                }
            };
            _connection.VerifyConnection();
            return Task.CompletedTask;
        }
        
        private void SubscribeJetStream(string exchange, CancellationToken token)
        {
            _connection.VerifyJetStream(exchange);
            
            var js = _connection.GetJetStream();
            var pullOptions = PullSubscribeOptions.Builder()
                .WithDurable($"{_options.Client}_{exchange}")
                .Build();
            var sub = js.PullSubscribe($"{exchange}.>", pullOptions);
            
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var message in sub.Fetch(_options.JetStreamBatchSize, _options.JetStreamWaitMills))
                    {
                        var eventName = GetEventName(message.Subject, exchange);        
                        await _eventProcessor.ProcessEvent(eventName, message.Data);
                        message.Ack();
                    }
                }
            }, token);
        }

        private void SubscribeNative(IConnection connection, string exchange, CancellationToken token)
        {
            connection.SubscribeAsync($"{exchange}.>", _options.Client, async (sender, args) =>
            {
                var eventName = GetEventName(args.Message.Subject, exchange);
                await _eventProcessor.ProcessEvent(eventName, args.Message.Data);
            });
        }

        private static string GetEventName(string subject, string exchange) =>
            subject.StartsWith(exchange)
                ? subject[(exchange.Length + 1)..]
                : subject;
    }
}