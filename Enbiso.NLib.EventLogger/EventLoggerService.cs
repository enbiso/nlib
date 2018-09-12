using System;
using System.Data.Common;
using System.Threading.Tasks;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventLogger
{
    /// <summary>
    /// EventLoger service
    /// </summary>
    public interface IEventLoggerService
    {
        EventLog AddEvent(IEvent @event);
        Task SaveEventAsync(IEvent @event, DbTransaction transaction = null);
        Task MarkEventAsPublishedAsync(IEvent @event);
        Task MarkEventAsFailedAsync(IEvent @event);
    }
    
    /// <inheritdoc />
    /// <summary>
    /// Event logger implementation
    /// </summary>
    public class EventLoggerService : IEventLoggerService
    {
        private readonly IEventLogRepo _repo;

        public EventLoggerService(IEventLogRepo repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Add @event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public EventLog AddEvent(IEvent @event)
        {
            var eventLogEntry = new EventLog(@event);
            return _repo.Add(eventLogEntry);
        }

        /// <summary>
        /// Save events
        /// </summary>
        /// <param name="event"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public Task SaveEventAsync(IEvent @event, DbTransaction transaction = null)
        {
            if (transaction != null)
                _repo.UseTransaction(transaction);

            var eventLogEntry = new EventLog(@event);
            _repo.Add(eventLogEntry);
            return _repo.SaveChangesAsync();
        }

        /// <summary>
        /// Mark @event as published
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task MarkEventAsPublishedAsync(IEvent @event)
        {
            var eventLogEntry = await _repo.FindByIdAsync(@event.Id);
            eventLogEntry.TimesSent++;
            eventLogEntry.State = EventState.Published;
            _repo.Update(eventLogEntry);
            await _repo.SaveChangesAsync();
        }

        /// <summary>
        /// Mark as failed
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task MarkEventAsFailedAsync(IEvent @event)
        {
            var eventLogEntry = await _repo.FindByIdAsync(@event.Id);            
            eventLogEntry.State = EventState.Failed;
            _repo.Update(eventLogEntry);
            await _repo.SaveChangesAsync();
        }
    }
}
