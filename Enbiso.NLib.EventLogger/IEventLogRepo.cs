using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventLogger
{
    /// <summary>
    /// Event log repository
    /// </summary>
    public interface IEventLogRepo
    {
        Task<EventLog> FindByIdAsync(Guid id);
        EventLog Add(EventLog eventLog);
        EventLog Update(EventLog eventLog);
        void UseTransaction(DbTransaction transaction);
        Task SaveChangesAsync();
    }
}