using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string eventName, byte[] data);
    }

    public class EventProcessor: IEventProcessor
    {
        private readonly Dictionary<string, List<IEventHandler>> _eventHandlers =
            new Dictionary<string, List<IEventHandler>>();
        
        public EventProcessor(IEnumerable<IEventHandler> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                var eventName = eventHandler.GetEventType().Name;
                if (_eventHandlers.TryGetValue(eventName, out var currentHandlers))
                {
                    currentHandlers.Add(eventHandler);
                    _eventHandlers[eventName] = currentHandlers;
                }
                else
                {
                    _eventHandlers.Add(eventName, new List<IEventHandler> {eventHandler});
                }
            }
        }

        public async Task ProcessEvent(string eventName, byte[] data)
        {
            var message = Encoding.UTF8.GetString(data);
            
            if(!_eventHandlers.ContainsKey(eventName)) return;
            
            var eventHandlers = _eventHandlers[eventName];
            foreach (var eventHandler in eventHandlers)
            {
                var eventType = eventHandler.GetEventType();
                var @event = JsonSerializer.Deserialize(message, eventType);
                await eventHandler.Handle(@event);
            }
        }
    }
}
