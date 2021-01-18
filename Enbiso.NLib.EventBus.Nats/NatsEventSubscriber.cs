using System;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NATS.Client;
using Polly;

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

        public Task Subscribe()
        {
            _connection.Connected += conn =>
            {
                foreach (var exchange in _options.Exchanges ?? new string[0])
                {
                    conn.SubscribeAsync($"{exchange}.>", _options.Client, async (sender, args) =>
                    {
                        var subject = args.Message.Subject;
                        var eventName = subject.StartsWith(exchange)
                            ? subject.Substring(exchange.Length + 1)
                            : subject;
                        await _eventProcessor.ProcessEvent(eventName, args.Message.Data);
                    });
                }
            };
            _connection.VerifyConnection();
            return Task.CompletedTask;
        }
    }
}