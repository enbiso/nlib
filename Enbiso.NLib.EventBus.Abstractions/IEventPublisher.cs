using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Event bus interface
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish @event
        /// </summary>
        /// <param name="event"></param>
        /// <param name="exchange"></param>
        /// <param name="cancellationToken"></param>
        Task Publish<T>(T @event, string exchange = null, CancellationToken cancellationToken = default) where T : IEvent;
    }
}
