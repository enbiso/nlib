using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public class EventService : IEventService
    {
        private readonly IEventBus _bus;

        public EventService(IEventBus bus)
        {
            _bus = bus;
        }

        public Task PublishToBus<T>(T @event, string exchange = null) where T : IEvent =>
            _bus.Publish(@event, exchange);
    }
}
