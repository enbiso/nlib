using System;
using Microsoft.Azure.ServiceBus;

namespace Enbiso.NLib.EventBus.ServiceBus
{
    /// <inheritdoc />
    /// <summary>
    /// Service bus persistence connection interface
    /// </summary>
    public interface IServiceBusPersisterConnection : IDisposable
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
}