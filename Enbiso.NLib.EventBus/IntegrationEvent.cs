using System;

namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Integration event abstraction
    /// </summary>
    public abstract class IntegrationEvent: IIntegrationEvent
    {
        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreationDate { get; }
    }
}
