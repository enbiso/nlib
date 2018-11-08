using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    public interface IRabbitMqPersistentConnection : IDisposable
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

    public class RabbitMqPersistentConnection : IRabbitMqPersistentConnection
    {
        private IConnection _connection;
        private bool _disposed;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqPersistentConnection> _logger;
        private readonly int _retryCount;
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Create default Rabbit connection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="option"></param>
        public RabbitMqPersistentConnection(
            ILogger<RabbitMqPersistentConnection> logger, 
            IOptions<RabbitMqOption> option)
        {
            var optVal = option.Value;
            _connectionFactory = new ConnectionFactory
            {
                HostName = optVal.Server ?? throw new ArgumentNullException(nameof(optVal.Server)),
            };
            if(!string.IsNullOrEmpty(optVal.Username))
                _connectionFactory.UserName = optVal.Username;
            if(!string.IsNullOrEmpty(optVal.Password))
                _connectionFactory.Password = optVal.Password;
            if (!string.IsNullOrEmpty(optVal.VirtualHost))
                _connectionFactory.VirtualHost = optVal.VirtualHost;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = optVal.RetryCount;
        }

        /// <inheritdoc />
        public bool IsConnected 
            => _connection != null && _connection.IsOpen && !_disposed;

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try
            {
                _connection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());                
            }
        }

        /// <inheritdoc />
        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");
            lock (_syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            _logger.LogWarning(ex.ToString());
                        });

                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (!IsConnected)
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                    return false;
                }

                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;

                _logger.LogInformation(
                    $"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
                return true;
            }
        }

        /// <inheritdoc />
        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        #region private methods

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }

        #endregion
    }
}
