using System.Data.Common;
using System.Threading.Tasks;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventLogger
{
    /// <summary>
    /// EventLogger service
    /// </summary>
    public interface IEventLoggerService
    {
        Task SaveEventAsync(IEvent @event, string eventType, DbTransaction transaction = null);
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
        /// Save events
        /// </summary>
        /// <param name="event"></param>
        /// <param name="eventType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public Task SaveEventAsync(IEvent @event, string eventType, DbTransaction transaction)
        {
            if (transaction != null)
                _repo.UseTransaction(transaction);

            var eventLogEntry = new EventLog(@event, eventType);
            
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
            var eventLogEntry = await _repo.FindByIdAsync(@event.EventId);
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
            var eventLogEntry = await _repo.FindByIdAsync(@event.EventId);            
            eventLogEntry.State = EventState.Failed;
            _repo.Update(eventLogEntry);
            await _repo.SaveChangesAsync();
        }
    }
}
