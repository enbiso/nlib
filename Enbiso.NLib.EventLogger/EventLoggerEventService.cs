using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventLogger
{
    public class EventLoggerEventService: IEventService
    {
        private readonly IEnumerable<IEventPublisher> _publishers;
        private readonly IEnumerable<IEventSubscriber> _subscribers;
        private readonly IEventLoggerService _service;

        public EventLoggerEventService(IEventLoggerService service, IEnumerable<IEventSubscriber> subscribers,
            IEnumerable<IEventPublisher> publishers)
        {
            _service = service;
            _subscribers = subscribers;
            _publishers = publishers;
        }

        public async Task PublishToBus<T>(T @event, string exchange, CancellationToken token) where T: IEvent
        {
            await _service.SaveEventAsync(@event);
            try
            {
                await Task.WhenAll(_publishers.Select(p => p.Publish(@event, exchange, token)));
                await _service.MarkEventAsPublishedAsync(@event);
            }
            catch (Exception)
            {
                await _service.MarkEventAsFailedAsync(@event);
                throw;
            }
        }

        public Task SubscribeAll(CancellationToken token = default) => 
            Task.WhenAll(_subscribers.Select(s => s.Subscribe(token)));
         
    }
}