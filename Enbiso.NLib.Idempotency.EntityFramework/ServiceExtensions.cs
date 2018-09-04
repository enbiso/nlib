using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Idempotency.EntityFramework
{
    public static class ServiceExtensions
    {
        public static void AddIdempotency<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddIdempotency();
            services.AddTransient<IRequestLogRepo, RequestLogRepo<TDbContext>>();
        }
    }
}