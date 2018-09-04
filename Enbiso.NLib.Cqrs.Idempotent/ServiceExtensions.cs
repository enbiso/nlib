using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Cqrs.Idempotent
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCqrsIdempotent(this IServiceCollection services, params Assembly[] assemblies)
        {
            var assemblyList = assemblies.ToList();
            assemblyList.Add(typeof(ServiceExtensions).Assembly);            
            services.AddCqrs(assemblies);            
            return services;
        }        
    }
}