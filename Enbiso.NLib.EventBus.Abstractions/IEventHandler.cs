using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Integration event handler
    /// </summary>
    public interface IEventHandler
    {
        string EventName { get; }
        Type EventType { get; }
        Task Handle(object @event);
    }
    
    /// <summary>
    /// Abstract event handler
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class EventHandler<TEvent>: IEventHandler where TEvent : class, IEvent
    {
        protected abstract Task Handle(TEvent @event);
        public string EventName => typeof(TEvent).Name;
        public Type EventType => typeof(TEvent);
        public Task Handle(object @event) => Handle(@event as TEvent);
    }
}
