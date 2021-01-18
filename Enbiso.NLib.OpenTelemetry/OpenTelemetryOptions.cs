using System.Collections.Generic;

namespace Enbiso.NLib.OpenTelemetry
{
    public class OpenTelemetryOptions
    {
        public string Endpoint { get; set; }
        public string ServiceName { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}