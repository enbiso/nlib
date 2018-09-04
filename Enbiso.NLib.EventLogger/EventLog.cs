using System;
using Enbiso.NLib.EventBus;
using Newtonsoft.Json;

namespace Enbiso.NLib.EventLogger
{
    public class EventLog
    {
        private EventLog() { }
        public EventLog(IEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventState.NotPublished;
            TimesSent = 0;
        }        
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        public EventState State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
    }
}
