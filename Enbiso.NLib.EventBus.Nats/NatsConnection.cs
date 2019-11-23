using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;

namespace Enbiso.NLib.EventBus.Nats
{
    public interface INatsConnection : IDisposable
    {
        bool IsConnected { get; }
        IConnection GetConnection();
        bool TryConnect();
    }

    public class NatsConnection: INatsConnection
    {
        private IConnection _connection;
        private readonly ConnectionFactory _factory;
        private readonly NatsOptions _options;
        private readonly ILogger _logger;
        private bool _disposed;

        public NatsConnection(ConnectionFactory factory, IOptions<NatsOptions> options, ILogger<NatsConnection> logger)
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
