using System;

namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Integration event abstraction
    /// </summary>
    public abstract class Event: IEvent
    {
        protected Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreationDate { get; }
    }
}
