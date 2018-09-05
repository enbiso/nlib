using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            assemblies.ToList().ForEach(services.AddServicesForAssembly);
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return services.AddServices(assemblies);
        }

        private static void AddServicesForAssembly(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var service = type.GetCustomAttribute<ServiceAttribute>();
                if (service == null) continue;
                // Get defined interfaces
                var interfaces = service.ServiceTypes.ToList();
                // If not specificed add implemented interfaces
                if (!interfaces.Any()) interfaces.AddRange(type.GetInterfaces());
                // If not then self bind
                if (!interfaces.Any()) interfaces.Add(type);

                switch (service.Lifetime)
                {
                    case ServiceLifetime.Transient:
                        interfaces.ForEach(@interface => services.AddTransient(@interface, type));
                        break;
                    case ServiceLifetime.Singleton:
                        interfaces.ForEach(@interface => services.AddSingleton(@interface, type));
                        break;
                    case ServiceLifetime.Scoped:
                        interfaces.ForEach(@interface => services.AddScoped(@interface, type));
                        break;
                    default:
                        throw new Exception("Unknown scope specified");
                }
            }
        }
    }
}
