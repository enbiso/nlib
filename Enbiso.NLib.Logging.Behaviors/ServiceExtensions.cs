﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Logging.Behaviors
{
    public static class ServiceExtensions
    {
        public static void AddLoggingBehavior(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }
    }
}