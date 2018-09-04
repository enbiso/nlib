using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Enbiso.NLib.Idempotency.EntityFramework
{
    /// <inheritdoc />
    /// <summary>
    /// Repo implementation in EF
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class RequestLogRepo<TDbContext>: IRequestLogRepo where TDbContext: DbContext
    {
        private readonly DbContext _context;

        public RequestLogRepo(TDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Set<RequestLog>().CountAsync(r => r.Id == id) > 0;
        }

        public RequestLog Add(RequestLog requestLog)
        {
            return _context.Set<RequestLog>().Add(requestLog).Entity;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
