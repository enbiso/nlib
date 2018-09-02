using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.Idempotency
{
    public interface IRequestLogRepo
    {
        Task<bool> ExistsAsync(Guid id);
        RequestLog Add(RequestLog requestLog);
        Task SaveChangesAsync();
    }
}