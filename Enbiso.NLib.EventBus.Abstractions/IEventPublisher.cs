using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish @event
        /// </summary>
        /// <param name="event"></param>
        /// <param name="exchange"></param>
        /// <param name="eventType"></param>
        /// <param name="cancellationToken"></param>
        Task Publish<TEvent>(TEvent @event, string exchange, string eventType, CancellationToken cancellationToken) where TEvent : IEvent;
    }
}