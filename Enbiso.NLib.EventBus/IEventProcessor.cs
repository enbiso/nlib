using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string eventName, byte[] data);
        void AddEventHandler(IEventHandler handler);
        event EventProcessorEventTypeAddedEventHandler EventTypeAdded;
    }
    
    public class EventProcessorEventTypeAddedEventArgs: EventArgs
    {
        public string EventName { get; set; }
        public Type EventType { get; set; }
    }
    
    public delegate void EventProcessorEventTypeAddedEventHandler(object sender,
        EventProcessorEventTypeAddedEventArgs e);
}