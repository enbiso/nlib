using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Cqrs.Idempotent
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCqrsIdempotent(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => !a.IsDynamic);
            return services.AddCqrsIdempotent(assembly);
        }

        public static IServiceCollection AddCqrsIdempotent(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            var assemblyList = assemblies.ToList();
            assemblyList.Add(typeof(ServiceExtensions).Assembly);
            services.AddCqrs(assemblies);
            services.AddSingleton<IIdempotentCommandBus, IdempotentCommandBus>();
            return services;
        }
    }
}