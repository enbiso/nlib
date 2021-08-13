using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public class EventService : IEventService
    {
        private readonly IEnumerable<IEventPublisher> _publishers;
        private readonly IEnumerable<IEventSubscriber> _subscribers;

        public EventService(IEnumerable<IEventPublisher> publishers, IEnumerable<IEventSubscriber> subscribers)
        {
            _publishers = publishers;
            _subscribers = subscribers;
        }

        public Task PublishToBus<T>(T @event, string exchange, CancellationToken token) where T : IEvent =>
            Task.WhenAll(_publishers.Select(p => p.Publish(@event, exchange, token)));

        public Task SubscribeAll(CancellationToken token) =>
            Task.WhenAll(_subscribers.Select(s => s.Subscribe(token)));
    }
}
