using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Enbiso.NLib.EventLogger.EntityFramework
{
    /// <inheritdoc />
    /// <summary>
    /// Entity Framework event log repo imelemtations
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class EntityEventLogRepo<TDbContext>: IEventLogRepo where TDbContext: DbContext
    {
        private readonly DbContext _context;

        public EntityEventLogRepo(TDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Find by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<EventLog> FindByIdAsync(Guid id)
        {
            return _context.Set<EventLog>().FirstOrDefaultAsync(e => e.EventId == id);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="eventLog"></param>
        /// <returns></returns>
        public EventLog Add(EventLog eventLog)
        {
            return _context.Set<EventLog>().Add(eventLog).Entity;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="eventLog"></param>
        /// <returns></returns>
        public EventLog Update(EventLog eventLog)
        {
            return _context.Set<EventLog>().Update(eventLog).Entity;
        }

        /// <summary>
        /// Use transaction
        /// </summary>
        /// <param name="transaction"></param>
        public void UseTransaction(DbTransaction transaction)
        {
            _context.Database.UseTransaction(transaction);
        }

        /// <summary>
        /// Save async
        /// </summary>
        /// <returns></returns>
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}