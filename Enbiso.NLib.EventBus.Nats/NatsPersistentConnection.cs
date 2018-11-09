using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using Options = NATS.Client.Options;

namespace Enbiso.NLib.EventBus.Nats
{
    public interface INatsPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        IConnection GetConnection();
        bool TryConnect();
    }

    public class NatsPersistentConnection: INatsPersistentConnection
    {
        private IConnection _connection;
        private readonly ConnectionFactory _factory;
        private readonly NatsOptions _options;
        private readonly ILogger _logger;
        private bool _disposed;

        public NatsPersistentConnection(ConnectionFactory factory, IOptions<NatsOptions> options, ILogger<NatsPersistentConnection> logger)
        {
            _logger = logger;
            _options = options.Value;
            _factory = factory;
        }

        public bool IsConnected
            => _connection != null && _connection.State == ConnState.CONNECTED && !_disposed;

        public IConnection GetConnection() => _connection;

        public bool TryConnect()
        {
            var opts = ConnectionFactory.GetDefaultOptions();
            opts.Servers = _options.Servers;
            opts.MaxReconnect = _options.RetryCount;
            if (!string.IsNullOrEmpty(_options.Username))
                opts.User = _options.Username;
            if (!string.IsNullOrEmpty(_options.Password))
                opts.Password = _options.Password;
            if (!string.IsNullOrEmpty(_options.Token))
                opts.Token = _options.Token;
            try
            {
                _connection = _factory.CreateConnection(opts);                
                return IsConnected;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to connect to NAT server");
                return false;
            }
        }

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

    }
}
