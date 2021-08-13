using System;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
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
            if(opts.Options == null) return;
            
            services.AddOpenTelemetryTracing(builder => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(opts.ServiceName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = opts.Options.Endpoint;
                    o.Headers = opts.Options.Headers;
                    o.TimeoutMilliseconds = opts.Options.TimeoutMilliseconds;
                    o.ExportProcessorType = opts.Options.ExportProcessorType;
                    o.BatchExportProcessorOptions = opts.Options.BatchExportProcessorOptions;
                }));
        }
    }
}