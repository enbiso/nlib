using System;

namespace Enbiso.NLib.EventBus.Abstractions
{
    public interface IIntegrationEvent
    {
        Guid Id { get; }
        DateTime CreationDate { get; }
    }
}