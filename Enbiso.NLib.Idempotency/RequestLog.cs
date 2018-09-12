using System;

namespace Enbiso.NLib.Idempotency
{
    /// <summary>
    /// Request Log
    /// </summary>
    public class RequestLog
    {
        public Guid Id { get; set; }        
        public string Name { get; set; }        
        public DateTime Time { get; set; }
        public string Response { get; set; }
    }
}
