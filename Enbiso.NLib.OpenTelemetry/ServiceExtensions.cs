using System;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Enbiso.NLib.OpenTelemetry
{
    public static class ServiceExtensions
    {
        public static void AddOpenTelemetry(this IServiceCollection services, Action<OpenTelemetryOptions> optBuilder)
        {
            services.AddOptions();
            
            optBuilder ??= _ => {};
            services.Configure(optBuilder);

            var opts = new OpenTelemetryOptions();
            optBuilder.Invoke(opts);
            if(string.IsNullOrEmpty(opts.Endpoint)) return;
            
            services.AddOpenTelemetryTracing(builder => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(opts.ServiceName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = opts.Endpoint;
                    o.Credentials = new SslCredentials();
                    o.Headers = new Metadata();
                    foreach (var (key, value) in opts.Headers)
                        o.Headers.Add(key, value);
                }));
        }
    }
}