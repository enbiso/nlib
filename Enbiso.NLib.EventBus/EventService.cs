using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public class EventService : IEventService
    {
        private readonly IEnumerable<IEventPublisher> _publishers;        

        public EventService(IEnumerable<IEventPublisher> publishers)
        {
            _publishers = publishers;
        }

        public Task PublishToBus<T>(T @event, string exchange, string eventType, CancellationToken token) where T : IEvent =>
            Task.WhenAll(_publishers.Select(p => p.Publish(@event, exchange, eventType, token)));
        
    }
}
