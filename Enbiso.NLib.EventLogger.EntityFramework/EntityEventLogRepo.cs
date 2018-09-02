using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Enbiso.NLib.EventLogger.EntityFramework
{
    public class EntityEventLogRepo<TDbContext>: IEventLogRepo where TDbContext: DbContext
    {
        private readonly DbContext _context;

        public EntityEventLogRepo(DbContext context)
        {
            _context = context;
        }

        public Task<EventLog> FindByIdAsync(Guid id)
        {
            return _context.Set<EventLog>().FirstOrDefaultAsync(e => e.EventId == id);
        }

        public EventLog Add(EventLog eventLog)
        {
            return _context.Set<EventLog>().Add(eventLog).Entity;
        }

        public EventLog Update(EventLog eventLog)
        {
            return _context.Set<EventLog>().Update(eventLog).Entity;
        }

        public void UseTransaction(DbTransaction transaction)
        {
            _context.Database.UseTransaction(transaction);
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}