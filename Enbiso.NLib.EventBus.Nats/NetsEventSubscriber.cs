using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Enbiso.NLib.EventBus.Nats
{
    public class NatsEventSubscriber: IEventSubscriber
    {
        private readonly INatsConnection _connection;
        private readonly NatsOptions _options;
        private readonly IEventProcessor _eventProcessor;

        public NatsEventSubscriber(INatsConnection connection, IOptions<NatsOptions> options, IEventProcessor eventProcessor)
        {
            _connection = connection;
            _eventProcessor = eventProcessor;
            _options = options.Value;
        }

        public void Initialize()
        {
            if (!_connection.TryConnect()) return;

            var conn = _connection.GetConnection();

            foreach (var exchange in _options.Exchanges)
            {
                conn.SubscribeAsync($"{exchange}.>", _options.Client, async (sender, args) =>
                {
                    var subject = args.Message.Subject;
                    var eventName = subject.StartsWith(exchange)
                        ? subject.Substring(exchange.Length + 1) : subject;                
                    await _eventProcessor.ProcessEvent(eventName, args.Message.Data);
                });
            }
        }

        public Task Subscribe(string eventName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UnSubscribe(string eventName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}