using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Event bus interface
    /// </summary>
    public interface IEventSubscriber: IDisposable
    {
        /// <summary>
        /// Subscribe to all events
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <returns></returns>
        Task Subscribe(CancellationToken token = default);
    }
}
