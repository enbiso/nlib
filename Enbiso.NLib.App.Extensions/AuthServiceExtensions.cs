using Enbiso.NLib.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.App.Extensions
{
    public static class AuthServiceExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IAppSettings settings)
        {                        
            services.AddAuthorization(); //.AddJsonFormatters();
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = settings.AuthUrl;
                    options.RequireHttpsMetadata = false;
                    options.EnableCaching = true;
                    options.ApiName = settings.Id;
                });
            return services;
        }        
    }
}