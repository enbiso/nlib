using System;
using System.Threading;
using System.Threading.Tasks;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventLogger
{
    public class EventLoggerEventService: IEventService
    {
        private readonly IEventPublisher _publisher;
        private readonly IEventLoggerService _service;

        public EventLoggerEventService(IEventPublisher publisher, IEventLoggerService service)
        {
            _publisher = publisher;
            _service = service;
        }

        public async Task PublishToBus<T>(T @event, string exchange, CancellationToken token) where T: IEvent
        {
            await _service.SaveEventAsync(@event);
            try
            {
                await _publisher.Publish(@event, exchange, token);
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