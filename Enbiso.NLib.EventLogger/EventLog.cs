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
            EventId = @event.EventId;
            CreationTime = @event.EventCreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventState.NotPublished;
            TimesSent = 0;
        }        
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        private string _state;

        public EventState State
        {
            get => Enum.TryParse<EventState>(_state, out var val) ? val : default(EventState);
            set => _state = value.ToString();
        }

        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
    }

    /// <summary>
    /// Event states
    /// </summary>
    public enum EventState
    {
        NotPublished = 0,
        Published = 1,
        Failed = 2
    }
}
