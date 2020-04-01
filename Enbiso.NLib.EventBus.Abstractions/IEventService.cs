using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventService
    {
        /// <summary>
        /// Publish to bus
        /// </summary>
        /// <param name="event"></param>
        /// <param name="exchange"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task PublishToBus<T>(T @event, string exchange = null, CancellationToken token = default) where T: IEvent;
    }
}
