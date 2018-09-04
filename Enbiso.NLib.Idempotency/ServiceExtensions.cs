using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Idempotency
{
    public static class ServiceExtensions
    {
        public static void AddIdempotency(this IServiceCollection services)
        {
            services.AddTransient<IRequestManager, RequestManager>();
        }
    }
}