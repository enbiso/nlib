using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public class EventSubscriptionService : IEventSubscriptionService
    {
        private readonly IEnumerable<IEventSubscriber> _subscribers;

        public EventSubscriptionService(IEnumerable<IEventSubscriber> subscribers)
        {
            _subscribers = subscribers;
        }

        public Task SubscribeAll(CancellationToken token) =>
            Task.WhenAll(_subscribers.Select(s => s.Subscribe(token)));
    }
}