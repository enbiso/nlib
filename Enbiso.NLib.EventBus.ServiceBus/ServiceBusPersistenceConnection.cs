using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.EventBus.ServiceBus
{
    /// <inheritdoc />
    /// <summary>
    /// Service bus persistence connection interface
    /// </summary>
    public interface IServiceBusPersistenceConnection : IDisposable
    {
        /// <summary>
        /// Connection string builder
        /// </summary>
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        /// <summary>
        /// Create Model
        /// </summary>
        /// <returns></returns>
        ITopicClient CreateModel();
    }

    /// <inheritdoc />
    /// <summary>
    /// Default service bus persistece connection
    /// </summary>
    public class ServiceBusPersistenceConnection : IServiceBusPersistenceConnection
    {
        private readonly ILogger<ServiceBusPersistenceConnection> _logger;
        private ITopicClient _topicClient;

        private bool _disposed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceBusConnectionStringBuilder"></param>
        /// <param name="logger"></param>
        public ServiceBusPersistenceConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder,
            ILogger<ServiceBusPersistenceConnection> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ServiceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ?? throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
            _topicClient = new TopicClient(ServiceBusConnectionStringBuilder, RetryPolicy.Default);
        }

        /// <inheritdoc />
        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        /// <inheritdoc />
        public ITopicClient CreateModel()
        {
            if (_topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(ServiceBusConnectionStringBuilder, RetryPolicy.Default);
            }
            return _topicClient;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}
