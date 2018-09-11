using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    /// <inheritdoc />
    /// <summary>
    /// Integration event handler with type event
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent: IEvent
    {
        Task Handle(TEvent @event);
    }

    /// <summary>
    /// Integration event handler
    /// </summary>
    public interface IEventHandler
    {

    }

    /// <inheritdoc />
    /// <summary>
    /// Dynamic integration event handler interface
    /// </summary>
    public interface IDynamicEventHandler: IEventHandler
    {
        /// <summary>
        /// Handle dynamic event
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        Task Handle(dynamic eventData);
    }
}
