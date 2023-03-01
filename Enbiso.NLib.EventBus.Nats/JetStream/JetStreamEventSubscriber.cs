using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NATS.Client.JetStream;

namespace Enbiso.NLib.EventBus.Nats.JetStream
{
    public class JetStreamEventSubscriber: IEventSubscriber
    {
        private readonly IJetStreamConnection _jsConnection;
        private readonly INatsConnection _natsConnection;
        private readonly IEventProcessor _eventProcessor;
        private readonly NatsOptions _options;

        public JetStreamEventSubscriber(IJetStreamConnection jsConnection, IOptions<NatsOptions> options,
            IEventProcessor eventProcessor, INatsConnection natsConnection)
        {
            _jsConnection = jsConnection;
            _eventProcessor = eventProcessor;
            _natsConnection = natsConnection;
            _options = options.Value;
        }

        public void Dispose()
        {
            _natsConnection?.Dispose();
            _jsConnection?.Dispose();
        }

        public Task Subscribe(CancellationToken token = default)
        {
            _jsConnection.Connected += _ =>
            {
                foreach (var exchange in _options.Exchanges ?? Array.Empty<string>())
                {
                    _jsConnection.VerifyJetStream(exchange);
            
                    var js = _jsConnection.GetJetStream();
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
            _natsConnection.VerifyConnection();
            return Task.CompletedTask;
        }

        private static string GetEventName(string subject, string exchange) =>
            subject.StartsWith(exchange)
                ? subject[(exchange.Length + 1)..]
                : subject;
    }
}