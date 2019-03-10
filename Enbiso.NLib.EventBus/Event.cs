using System;

namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Integration event abstraction
    /// </summary>
    public abstract class Event : IEvent
    {
        protected Event()
        {
            EventId = Guid.NewGuid();
            EventCreationDate = DateTime.UtcNow;
        }

        public Guid EventId { get; }
        public DateTime EventCreationDate { get; }
    }
}
