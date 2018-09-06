using System;

namespace Enbiso.NLib.EventBus
{
    public interface IIntegrationEvent
    {
        Guid Id { get; }
        DateTime CreationDate { get; }
    }
}