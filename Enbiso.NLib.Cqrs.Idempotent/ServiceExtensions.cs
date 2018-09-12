using System.Reflection;
using Enbiso.NLib.Idempotency;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Cqrs.Idempotent
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCqrsIdempotent(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
            return services.AddCqrsIdempotent(assembly);
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