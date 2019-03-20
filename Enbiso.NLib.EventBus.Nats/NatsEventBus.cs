using System;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using Newtonsoft.Json;
using Polly;

namespace Enbiso.NLib.EventBus.Nats
{
    public class NatsEventBus : IEventBus
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly ILogger _logger;
        private readonly NatsOptions _options;
        private readonly INatsPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionsManager _subscriptionsManager;

        public NatsEventBus(IOptions<NatsOptions> options, INatsPersistentConnection persistentConnection,
            IEventProcessor eventProcessor, IEventBusSubscriptionsManager subscriptionsManager,
            ILogger<NatsEventBus> logger)
        {
            _persistentConnection = persistentConnection;
            _eventProcessor = eventProcessor;
            _subscriptionsManager = subscriptionsManager;
            _logger = logger;
            _options = options.Value;
        }

        public void Initialize()
        {
            if (!_persistentConnection.TryConnect()) return;

            var conn = _persistentConnection.GetConnection();            
            conn.SubscribeAsync($"{_options.Exchange}.>", _options.Client, async (sender, args) =>
            {
                var subject = args.Message.Subject;
                var eventName = subject.StartsWith(_options.Exchange)
                    ? subject.Substring(_options.Exchange.Length + 1) : subject;                
                await _eventProcessor.ProcessEvent(eventName, args.Message.Data);
            });
        }

        public void Publish(IEvent @event)
        {
            if (!_persistentConnection.IsConnected && !_persistentConnection.TryConnect()) return;
            var conn = _persistentConnection.GetConnection();
            var eventName = $"{_options.Exchange}.{@event.GetType().Name}";
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var policy = Policy.Handle<NATSTimeoutException>()
                .Or<SocketException>()
                .WaitAndRetry(_options.PublishRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => { _logger.LogWarning(ex.ToString()); });

            policy.Execute(() => {
                conn.Publish(eventName, body);
                conn.Flush();
            });
        }

        /// <inheritdoc />
        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            _subscriptionsManager.AddSubscription<TEvent, TEventHandler>();
        }

        /// <inheritdoc />
        public void Subscribe<TEvent>() where TEvent : IEvent
        {
            Subscribe<TEvent, IEventHandler<TEvent>>();
        }

        /// <inheritdoc />
        public void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler
        {
            _subscriptionsManager.AddDynamicSubscription<TEventHandler>(eventName);
        }

        /// <inheritdoc />
        public void UnsubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler
        {
            _subscriptionsManager.RemoveDynamicSubscription<TEventHandler>(eventName);
        }


        /// <inheritdoc />
        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            _subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();
        }
    }
}