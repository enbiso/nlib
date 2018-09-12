using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.App.Extensions
{
    public static class AuthServiceExtensions
    {
        /// <summary>
        /// Add auth with configuration section
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddAuth(configuration.Bind);
        }

        /// <summary>
        /// Add auth with settings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuth(this IServiceCollection services, Action<AuthOptions> settings)
        {                        
            var authOpt = new AuthOptions();
            settings?.Invoke(authOpt);

            services.AddAuthorization();
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authOpt.Authority;
                    options.RequireHttpsMetadata = authOpt.RequireHttpsMetadata;
                    options.EnableCaching = authOpt.EnableCaching;
                    options.ApiName = authOpt.ApiName;
                });
            return services;
        }        
    }

    public class AuthOptions
    {
        public bool RequireHttpsMetadata { get; set; } = false;
        public bool EnableCaching { get; set; } = true;
        public string Authority { get; set; } = "https://id.enbiso.com";
        public string ApiName { get; set; }
    }
}