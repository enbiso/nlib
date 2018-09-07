using System;
using System.Threading.Tasks;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventLogger
{
    public class EventLoggerEventService: IEventService
    {
        protected readonly IEventBus Bus;
        protected readonly IEventLoggerService Service;

        public EventLoggerEventService(IEventBus bus, IEventLoggerService service)
        {
            Bus = bus;
            Service = service;
        }

        public async Task PublishToBus(IEvent @event)
        {
            await PrePublish(@event);
            try
            {
                Bus.Publish(@event);
                await Service.MarkEventAsPublishedAsync(@event);
            }
            catch (Exception)
            {
                await Service.MarkEventAsFailedAsync(@event);
                throw;
            }
        }

        protected virtual Task PrePublish(IEvent @event)
        {
            return Service.SaveEventAsync(@event);
        }
    }
}