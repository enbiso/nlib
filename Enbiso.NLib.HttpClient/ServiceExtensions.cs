using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.HttpClient
{
    public static class ServiceExtensions
    {
        public static void AddHttpClient(this IServiceCollection services)
        {            
            services.AddTransient<IHttpClient, StandardHttpClient>();
        }

        public static void AddResilientHttpClient(this IServiceCollection services)
        {
            services.AddTransient<IHttpClient, ResilientHttpClient>();
        }
    }
}