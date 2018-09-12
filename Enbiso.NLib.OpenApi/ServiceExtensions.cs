using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Enbiso.NLib.OpenApi
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add open api with configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddOpenApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenApi(configuration.Bind);
        }

        /// <summary>
        /// Add open Api with builder
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optBuilder"></param>
        public static void AddOpenApi(this IServiceCollection services, Action<OpenApiOptions> optBuilder)
        {            
            var opts = new OpenApiOptions();
            optBuilder?.Invoke(opts);
            
            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<SchemaExtensionFilter>();
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("swagger", new Info
                {
                    Title = opts.Id?.ToUpper(),
                    Version = opts.Version,
                    Description = opts.Description
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{opts.Authority}/connect/authorize",
                    TokenUrl = $"{opts.Authority}/connect/token",
                    Scopes = new Dictionary<string, string>
                    {
                        {opts.Id, opts.Id?.ToUpper()}
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
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => { swaggerDoc.BasePath = settings.BasePath; });
            });
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"{settings.BasePath}/swagger.json", settings.Id?.ToUpper()); });
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
    }
}