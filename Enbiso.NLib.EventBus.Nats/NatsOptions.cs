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
        /// Default brokers to publish messages
        /// </summary>
        public string PublishExchange { get; set; }
        /// <summary>
        /// Client brokers to get subscribed
        /// </summary>        
        public string[] Exchanges { get; set; }
        /// <summary>
        /// Connection token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Use NATS JetStream
        /// </summary>
        public bool JetStreamEnable { get; set; } = false;
        /// <summary>
        /// Jet Stream poll batch size
        /// </summary>
        public int JetStreamBatchSize { get; set; } = 1;
        /// <summary>
        /// Jet Stream poll wait ms
        /// </summary>
        public int JetStreamWaitMills { get; set; } = 1000;
    }
}
