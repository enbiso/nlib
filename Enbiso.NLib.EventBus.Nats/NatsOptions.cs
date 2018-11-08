using System.Collections.Generic;

namespace Enbiso.NLib.EventBus.Nats
{
    public class NatsOptions
    {
        public string[] Servers { get; set; }
        /// <summary>
        /// Rabbit server connection retries
        /// </summary>
        public int RetryCount { get; set; } = 5;
        /// <summary>
        /// Message publish retries
        /// </summary>
        public int PublishRetryCount { get; set; } = 5;
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Client (Queue) name
        /// </summary>
        public string Client { get; set; }
        /// <summary>
        /// Client broker name
        /// </summary>
        public string Exchange { get; set; }
    }
}
