using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.Validation.Behaviors
{
    public static class ServiceExtensions
    {
        public static void AddValidationBehavior(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        }
    }
}