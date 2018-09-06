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
        EventLog AddEvent(IIntegrationEvent integrationEvent);
        Task SaveEventAsync(IIntegrationEvent integrationEvent, DbTransaction transaction);
        Task MarkEventAsPublishedAsync(IIntegrationEvent integrationEvent);
        Task MarkEventAsFailedAsync(IIntegrationEvent integrationEvent);
    }
    
    /// <inheritdoc />
    /// <summary>
    /// IntegrationEvent logger implementation
    /// </summary>
    public class EventLoggerService : IEventLoggerService
    {
        private readonly IEventLogRepo _repo;

        public EventLoggerService(IEventLogRepo repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Add integrationEvent
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public EventLog AddEvent(IIntegrationEvent integrationEvent)
        {
            var eventLogEntry = new EventLog(integrationEvent);
            return _repo.Add(eventLogEntry);
        }

        /// <summary>
        /// Save events
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public Task SaveEventAsync(IIntegrationEvent integrationEvent, DbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), $"A {typeof(DbTransaction).FullName} is required as a pre-requisite to save the integrationEvent.");
            }

            var eventLogEntry = new EventLog(integrationEvent);
            _repo.UseTransaction(transaction);
            _repo.Add(eventLogEntry);
            return _repo.SaveChangesAsync();
        }

        /// <summary>
        /// Mark integrationEvent as published
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task MarkEventAsPublishedAsync(IIntegrationEvent integrationEvent)
        {
            var eventLogEntry = await _repo.FindByIdAsync(integrationEvent.Id);
            eventLogEntry.TimesSent++;
            eventLogEntry.State = EventState.Published;
            _repo.Update(eventLogEntry);
            await _repo.SaveChangesAsync();
        }

        /// <summary>
        /// Mark as failed
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task MarkEventAsFailedAsync(IIntegrationEvent integrationEvent)
        {
            var eventLogEntry = await _repo.FindByIdAsync(integrationEvent.Id);            
            eventLogEntry.State = EventState.Failed;
            _repo.Update(eventLogEntry);
            await _repo.SaveChangesAsync();
        }
    }
}
