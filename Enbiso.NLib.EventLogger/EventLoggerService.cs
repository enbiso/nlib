using System;
using System.Data.Common;
using System.Threading.Tasks;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventLogger
{
    public interface IEventLoggerService
    {
        EventLog AddEvent(IEvent @event);
        Task SaveEventAsync(IEvent @event, DbTransaction transaction);
        Task MarkEventAsPublishedAsync(IEvent @event);
    }
    
    public class EventLoggerService : IEventLoggerService
    {
        private readonly IEventLogRepo _repo;

        public EventLoggerService(IEventLogRepo repo)
        {
            _repo = repo;
        }

        public EventLog AddEvent(IEvent @event)
        {
            var eventLogEntry = new EventLog(@event);
            return _repo.Add(eventLogEntry);
        }

        public Task SaveEventAsync(IEvent @event, DbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), $"A {typeof(DbTransaction).FullName} is required as a pre-requisite to save the event.");
            }

            var eventLogEntry = new EventLog(@event);
            _repo.UseTransaction(transaction);
            _repo.Add(eventLogEntry);
            return _repo.SaveAsync();
        }

        public async Task MarkEventAsPublishedAsync(IEvent @event)
        {
            var eventLogEntry = await _repo.FindByIdAsync(@event.Id);
            eventLogEntry.TimesSent++;
            eventLogEntry.State = EventState.Published;
            _repo.Update(eventLogEntry);
            await _repo.SaveAsync();
        }
    }
}
