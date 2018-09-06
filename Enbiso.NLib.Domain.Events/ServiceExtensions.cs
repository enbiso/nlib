using System;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Domain.Events
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDomainEvents(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => !a.IsDynamic);
            return services.AddDomainEvents(assembly);
        }

        public static IServiceCollection AddDomainEvents(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);
            services.AddSingleton<IDomainEventBus, DomainEventBus>();
            return services;
        }
    }
}
