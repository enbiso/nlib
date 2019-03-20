using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        /// <param name="authorizationBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration, Action<AuthorizationOptions> authorizationBuilder = null)
        {
            return services.AddAuth(configuration.Bind, authorizationBuilder);
        }

        /// <summary>
        /// Add auth with authBuilder
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authBuilder"></param>
        /// <param name="authorizationBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuth(this IServiceCollection services, Action<AuthOptions> authBuilder, Action<AuthorizationOptions> authorizationBuilder = null)
        {                        
            var authOpt = new AuthOptions();
            authBuilder?.Invoke(authOpt);

            if (authorizationBuilder == null)
                services.AddAuthorization();
            else
                services.AddAuthorization(authorizationBuilder);

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authOpt.Authority;
                    options.RequireHttpsMetadata = authOpt.RequireHttpsMetadata;
                    options.EnableCaching = authOpt.EnableCaching;
                    options.ApiName = authOpt.ApiName;
                    options.RoleClaimType = ClaimTypes.Role;
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

    public class PolicyOptions
    {
        internal readonly Dictionary<string, AuthorizationPolicy> Policies = new Dictionary<string, AuthorizationPolicy>();
        internal readonly Dictionary<string, Action<AuthorizationPolicyBuilder>> ConfigPolicies = new Dictionary<string, Action<AuthorizationPolicyBuilder>>();

        public void AddPolicy(string name, AuthorizationPolicy policy) => Policies.Add(name, policy);
        public void AddPolicy(string name, Action<AuthorizationPolicyBuilder> configPolicy) => ConfigPolicies.Add(name, configPolicy);
    }
}