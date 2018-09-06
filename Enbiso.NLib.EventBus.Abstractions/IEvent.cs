using System;

namespace Enbiso.NLib.EventBus
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime CreationDate { get; }
    }
}