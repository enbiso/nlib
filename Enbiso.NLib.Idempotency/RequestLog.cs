using System;

namespace Enbiso.NLib.Idempotency
{
    public class RequestLog
    {
        public Guid Id { get; set; }        
        public string Name { get; set; }        
        public DateTime Time { get; set; }
    }
}
