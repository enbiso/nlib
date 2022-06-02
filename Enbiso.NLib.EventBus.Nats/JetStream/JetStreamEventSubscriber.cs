using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NATS.Client.JetStream;

namespace Enbiso.NLib.EventBus.Nats.JetStream
{
    public class JetStreamEventSubscriber: IEventSubscriber
    {
        private readonly IJetStreamConnection _connection;
        private readonly IEventProcessor _eventProcessor;
        private readonly NatsOptions _options;

        public JetStreamEventSubscriber(IJetStreamConnection connection, IOptions<NatsOptions> options, IEventProcessor eventProcessor)
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
            _connection.Connected += _ =>
            {
                foreach (var exchange in _options.Exchanges ?? Array.Empty<string>())
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
                            foreach (var message in sub.Fetch(_options.PollBatchSize, _options.PollWaitMills))
                            {
                                var eventName = GetEventName(message.Subject, exchange);        
                                await _eventProcessor.ProcessEvent(eventName, message.Data);
                                message.Ack();
                            }
                        }
                    }, token);
                }
            };
            return Task.CompletedTask;
        }

        private static string GetEventName(string subject, string exchange) =>
            subject.StartsWith(exchange)
                ? subject[(exchange.Length + 1)..]
                : subject;
    }
}