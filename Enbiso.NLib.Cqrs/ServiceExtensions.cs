using System;
using System.Linq;
using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Cqrs
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, bool autoLoadHandlers = true)
        {
            if (!autoLoadHandlers) return services.AddCqrs(new Assembly[0]);
            return services.AddCqrs(Assembly.GetCallingAssembly());
        }

        public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services.All(s => s.ServiceType != typeof(IMediator)))
                services.AddMediatR(assemblies);

            services.AddScoped<ICommandBus, CommandBus>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProcessorBehaviour<,>));
            return services;
        }
    }
}