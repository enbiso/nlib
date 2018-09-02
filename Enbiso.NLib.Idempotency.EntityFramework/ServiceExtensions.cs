using Enbiso.NLib.Idempotency.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Idempotency
{
    public static class ServiceExtensions
    {
        public static void AddIdempotency<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddTransient<IRequestManager, RequestManager<TDbContext>>();
        }
    }
}