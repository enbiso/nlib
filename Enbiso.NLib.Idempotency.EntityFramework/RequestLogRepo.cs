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

        public async Task<RequestLog> FindAsync(Guid id)
        {
            return await _context.Set<RequestLog>().FirstOrDefaultAsync(rl => rl.Id == id);
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
