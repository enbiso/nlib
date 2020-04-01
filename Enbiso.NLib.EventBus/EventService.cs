using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public class EventService : IEventService
    {
        private readonly IEventPublisher _publisher;

        public EventService(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task PublishToBus<T>(T @event, string exchange, CancellationToken token) where T : IEvent =>
            _publisher.Publish(@event, exchange, token);
    }
}
