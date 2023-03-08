using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Enbiso.NLib.OpenApi
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add open Api with builder
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optBuilder"></param>
        public static void AddOpenApi(this IServiceCollection services, Action<OpenApiOptions> optBuilder)
        {
            services.AddOptions();
            
            optBuilder ??= _ => {};
            services.Configure(optBuilder);

            var opts = new OpenApiOptions();
            optBuilder.Invoke(opts);

            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<SchemaExtensionFilter>();
                c.SwaggerDoc("swagger", new OpenApiInfo
                {
                    Title = opts.Id?.ToUpper(),
                    Version = opts.Version,
                    Description = opts.Description
                });
                if (string.IsNullOrEmpty(opts.Authority)) return;

                var scopes = opts.ExtraScopes?.ToDictionary(s => s, s => s.ToUpper()) ??
                             new Dictionary<string, string>();

                if (opts.Id != null) scopes.Add(opts.Id, opts.Description);
                
                c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                   Type = SecuritySchemeType.OAuth2,
                   Flows = new OpenApiOAuthFlows
                   {
                       Implicit = new OpenApiOAuthFlow
                       {
                           AuthorizationUrl = new Uri($"{opts.Authority}/connect/authorize"),
                           TokenUrl = new Uri($"{opts.Authority}/connect/token"),
                           Scopes = scopes
                       }
                   },
                });
               
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "OAuth2"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        scopes.Keys.ToList()
                    }
                });
            });
        }

        public static void UseOpenApi(this IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetRequiredService<IOptions<OpenApiOptions>>().Value;
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/{documentName}.json";
                c.PreSerializeFilters.Add(((document, request) =>
                {
                    document.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer
                        {
                            Url = settings.BasePath
                        }
                    };
                }));
            });
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"{settings.BasePath}swagger.json", settings.Id.ToUpper());});
        }
    }

    public class OpenApiOptions
    {
        /// <summary>
        /// Api ID
        /// </summary>
        public string Id { get; set; }        
        /// <summary>
        /// API Version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// API description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Swagger authority
        /// </summary>
        public string Authority { get; set; }
        /// <summary>
        /// Api base path
        /// </summary>
        public string BasePath { get; set; }
        /// <summary>
        /// Extra scopes to add to Swagger UI
        /// </summary>
        public string[] ExtraScopes { get; set; }
    }
}
