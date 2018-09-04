using System.Collections.Generic;
using Enbiso.NLib.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Enbiso.NLib.OpenApi
{
    public static class ServiceExtensions
    {
        public static void AddOpenApi(this IServiceCollection services, IAppSettings settings)
        {                        
            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<SchemaExtensionFilter>();
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("swagger", new Info
                {
                    Title = settings.Id?.ToUpper(),
                    Version = settings.Version,
                    Description = settings.Description
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{settings.AuthUrl}/connect/authorize",
                    TokenUrl = $"{settings.AuthUrl}/connect/token",
                    Scopes = new Dictionary<string, string>
                    {
                        {settings.Id, settings.Id?.ToUpper()}
                    }
                });
            });
        }

        public static void UseOpenApi(this IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetRequiredService<IOptions<AppSettings>>().Value;
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/{documentName}.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => { swaggerDoc.BasePath = settings.BasePath; });
            });
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"{settings.BasePath}/swagger.json", settings.Id?.ToUpper()); });
        }
    }
}