using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.EventBus
{
    public class EventProcessor: IEventProcessor
    {
        private readonly Dictionary<string, List<IEventHandler>> _subscriptions = new();
        
        private readonly IEnumerable<IEventHandler> _eventHandlers;
        private readonly ILogger _logger;
        
        public EventProcessor(IEnumerable<IEventHandler> eventHandlers, ILogger<EventProcessor> logger)
        {
            _eventHandlers = eventHandlers;
            _logger = logger;
        }

        public async Task ProcessEvent(string eventName, byte[] data)
        {
            var message = Encoding.UTF8.GetString(data);
            
            if(!_subscriptions.ContainsKey(eventName)) return;
            
            var eventHandlers = _subscriptions[eventName];
            foreach (var eventHandler in eventHandlers)
            {
                var eventType = eventHandler.GetEventType();
                var @event = JsonSerializer.Deserialize(message, eventType);
                if (@event is IEvent iEvent)
                    _logger.LogTrace("Processing {EventType} {EventId}", eventType, iEvent.EventId);
                else 
                    _logger.LogTrace("Processing {EventType}", eventType);

                await eventHandler.Handle(@event);
            }
        }

        public void Setup(Action<string> onAddSubscription)
        {
            foreach (var eventHandler in _eventHandlers)
            {
                var eventName = eventHandler.GetEventType().Name;
                
                onAddSubscription?.Invoke(eventName);
                
                if (_subscriptions.TryGetValue(eventName, out var currentHandlers))
                {
                    currentHandlers.Add(eventHandler);
                    _subscriptions[eventName] = currentHandlers;
                }
                else
                {
                    _subscriptions.Add(eventName, new List<IEventHandler> {eventHandler});
                }
            }
        }
    }
}
