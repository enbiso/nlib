using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventSubscriptionService
    {
        /// <summary>
        /// Subscribe all event processors
        /// </summary>
        /// <returns></returns>
        Task SubscribeAll(CancellationToken token = default);
    }
}