using System;
using Enbiso.NLib.EventBus.Abstractions;

namespace Enbiso.NLib.EventBus
{
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
