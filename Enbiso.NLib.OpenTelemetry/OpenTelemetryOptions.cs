using System.Collections.Generic;
using OpenTelemetry.Exporter;

namespace Enbiso.NLib.OpenTelemetry
{
    public class OpenTelemetryOptions
    {
        public string ServiceName { get; set; }
        public OtlpExporterOptions Options { get; set; }
    }
}