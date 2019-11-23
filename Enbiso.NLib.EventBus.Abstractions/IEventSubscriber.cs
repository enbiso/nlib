using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Event bus interface
    /// </summary>
    public interface IEventSubscriber
    {
        /// <summary>
        /// Initialize event bus with subscriptions        
        /// </summary>        
        void Initialize();

        /// <summary>
        /// Subscribe
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Subscribe(string eventName, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// UnSubscribe
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UnSubscribe(string eventName, CancellationToken cancellationToken = default);
    }
}
