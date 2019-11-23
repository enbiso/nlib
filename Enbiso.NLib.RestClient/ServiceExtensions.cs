using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.RestClient
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRestClient(this IServiceCollection services, Action<RestClientOptions> optSetup)
        {
            services.AddOptions();
            services.Configure(optSetup);
            services.AddSingleton<IRestClient, RestClient>();
            return services;
        }
    }
    
    public class RestClientOptions
    {
        public string TokenEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public IDictionary<string, string> Services { get; set; }
        public string GrantType { get; set; } = "client_credentials";
    }
}