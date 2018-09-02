using System;

namespace Enbiso.NLib.EventBus
{
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
