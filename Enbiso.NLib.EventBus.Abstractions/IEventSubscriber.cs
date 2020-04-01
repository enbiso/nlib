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
        /// <returns></returns>
        Task Subscribe();
    }
}
