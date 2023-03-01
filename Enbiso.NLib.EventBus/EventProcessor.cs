using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.EventBus
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string eventName, byte[] data);
        void AddEventHandler(IEventHandler handler);
    }
    
    public class EventProcessor: IEventProcessor
    {
        private readonly List<IEventHandler> _eventHandlers = new();
        private readonly ILogger _logger;
        private readonly IEventTypeManager _eventTypeManager;

        public EventProcessor(IEnumerable<IEventHandler> eventHandlers, ILogger<EventProcessor> logger,
            IEventTypeManager eventTypeManager)
        {
            _logger = logger;
            _eventTypeManager = eventTypeManager;
            foreach (var eventHandler in eventHandlers)
                AddEventHandler(eventHandler);
        }

        public async Task ProcessEvent(string eventName, byte[] data)
        {
            var message = Encoding.UTF8.GetString(data);
            
            foreach (var eventHandler in _eventHandlers.Where(h => h.IsValidFor(eventName)))
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

        public void AddEventHandler(IEventHandler handler)
        {
            _eventHandlers.Add(handler);
            _eventTypeManager.Add(handler.EventType);
        }
    }


}
