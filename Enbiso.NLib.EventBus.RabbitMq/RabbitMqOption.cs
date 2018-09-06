namespace Enbiso.NLib.EventBus.RabbitMq
{
    public class RabbitMqOption
    {
        /// <summary>
        /// Server host
        /// </summary>
        public string Server { get; set; } = "localhost";
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Virtual hosts
        /// </summary>
        public string VirtualHost { get; set; }
        /// <summary>
        /// Rabbit server connection retries
        /// </summary>
        public int RetryCount { get; set; } = 5;
        /// <summary>
        /// Message publish retries
        /// </summary>
        public int PublishRetryCount { get; set; } = 5;
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