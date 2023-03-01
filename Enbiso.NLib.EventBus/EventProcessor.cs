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
        private readonly ILogger _logger;

        public EventProcessor(IEnumerable<IEventHandler> eventHandlers, ILogger<EventProcessor> logger)
        {
            foreach (var eventHandler in eventHandlers)
            {
                AddEventHandler(eventHandler);
            }
            _logger = logger;
        }

        public async Task ProcessEvent(string eventName, byte[] data)
        {
            var message = Encoding.UTF8.GetString(data);
            
            if(!_subscriptions.ContainsKey(eventName)) return;
            
            var eventHandlers = _subscriptions[eventName];
            foreach (var eventHandler in eventHandlers)
            {
                var eventType = eventHandler.EventType;
                var @event = JsonSerializer.Deserialize(message, eventType);

                var eventId = @event is IEvent iEvent ? iEvent.EventId : Guid.Empty;
                try
                {
                    _logger.LogTrace("Processing {EventType} {EventId}", eventType, eventId);
                    await eventHandler.Handle(@event);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error Processing {EventType} {EventId}", eventType, eventId);
                }
            }
        }

        public event EventProcessorEventTypeAddedEventHandler EventTypeAdded;
        public void AddEventHandler(IEventHandler eventHandler)
        {
            var eventName = eventHandler.EventName;
            if (_subscriptions.TryGetValue(eventName, out var currentHandlers))
            {
                currentHandlers.Add(eventHandler);
                _subscriptions[eventName] = currentHandlers;
            }
            else
            {
                _subscriptions.Add(eventName, new List<IEventHandler> {eventHandler});
                EventTypeAdded?.Invoke(this, new EventProcessorEventTypeAddedEventArgs
                {
                    EventType = eventHandler.EventType,
                    EventName = eventName
                });
            }
        }
    }
}
