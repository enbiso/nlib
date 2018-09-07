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

        public Task PublishToBus(IEvent @event)
        {
            _bus.Publish(@event);
            return Task.CompletedTask;
        }
    }
}
