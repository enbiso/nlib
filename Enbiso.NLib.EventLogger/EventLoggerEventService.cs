using System;
using System.Threading.Tasks;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventLogger
{
    public class EventLoggerEventService: IEventService
    {
        private readonly IEventBus _bus;
        private readonly IEventLoggerService _service;

        public EventLoggerEventService(IEventBus bus, IEventLoggerService service)
        {
            _bus = bus;
            _service = service;
        }

        public async Task PublishToBus<T>(T @event, string exchange = null) where T: IEvent
        {
            await _service.SaveEventAsync(@event);
            try
            {
                await _bus.Publish(@event, exchange);
                await _service.MarkEventAsPublishedAsync(@event);
            }
            catch (Exception)
            {
                await _service.MarkEventAsFailedAsync(@event);
                throw;
            }
        }
    }
}