using System.Reflection;
using Enbiso.NLib.Idempotency;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Cqrs.Idempotent
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCqrsIdempotent(this IServiceCollection services, bool autoLoadHandlers = true)
        {
            if (autoLoadHandlers) return services.AddCqrsIdempotent(Assembly.GetCallingAssembly());
            return services.AddCqrsIdempotent(new Assembly[0]);
        }

        public static IServiceCollection AddCqrsIdempotent(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddIdempotency();
            services.AddCqrs(assemblies);
            services.AddScoped<IIdempotentCommandBus, IdempotentCommandBus>();
            return services;
        }
    }
}