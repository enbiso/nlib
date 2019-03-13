using System;

namespace Enbiso.NLib.EventBus
{
    public interface IEvent
    {
        Guid EventId { get; }
        DateTime EventCreationDate { get; }
    }
}