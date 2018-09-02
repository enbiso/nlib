using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventLogger
{
    public interface IEventLogRepo
    {
        Task<EventLog> FindByIdAsync(Guid id);
        EventLog Add(EventLog eventLog);
        EventLog Update(EventLog eventLog);
        void UseTransaction(DbTransaction transaction);
        Task SaveAsync();
    }
}