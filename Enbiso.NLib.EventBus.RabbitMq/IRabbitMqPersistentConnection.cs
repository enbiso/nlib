using System;
using RabbitMQ.Client;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    public interface IRabbitMqPersistentConnection: IDisposable
    {
        /// <summary>
        /// Check if connection to rabbit is success
        /// </summary>
        bool IsConnected { get; }
        
        /// <summary>
        /// Try connect to rabbit
        /// </summary>
        /// <returns></returns>
        bool TryConnect();
        
        /// <summary>
        /// Create Model
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}