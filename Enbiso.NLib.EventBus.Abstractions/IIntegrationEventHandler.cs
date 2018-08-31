using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus.Abstractions
{
    /// <summary>
    /// Integration event handler with type event
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler where TEvent: IIntegrationEvent
    {
        Task Handle(TEvent @event);
    }

    /// <summary>
    /// Integration event handler
    /// </summary>
    public interface IIntegrationEventHandler
    {

    }

    /// <summary>
    /// Dynamic integration event handler interface
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        /// <summary>
        /// Handle dynamic event
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        Task Handle(dynamic eventData);
    }
}
