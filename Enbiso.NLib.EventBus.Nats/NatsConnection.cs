using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.Rx;
using Polly;
using Options = NATS.Client.Options;

namespace Enbiso.NLib.EventBus.Nats
{
    public interface INatsConnection : IDisposable
    {
        IConnection GetConnection();
        void VerifyConnection();
        event ConnectedEventHandler Connected;
    }

    public delegate void ConnectedEventHandler(IConnection connection);

    public class NatsConnection: INatsConnection
    {
        private IConnection _connection;
        private readonly ConnectionFactory _factory;
        private readonly Options _opts;
        private readonly ILogger _logger;
        private bool _disposed;
        public event ConnectedEventHandler Connected;

        public NatsConnection(ConnectionFactory factory, IOptions<NatsOptions> options, ILogger<NatsConnection> logger)
        {
            _logger = logger;
            _factory = factory;
            
            var settings = options.Value;
            _opts = ConnectionFactory.GetDefaultOptions();
            _opts.Servers = settings.Servers;
            
            _opts.MaxReconnect = settings.RetryCount;
            if (!string.IsNullOrEmpty(settings.Username))
                _opts.User = settings.Username;
            if (!string.IsNullOrEmpty(settings.Password))
                _opts.Password = settings.Password;
            if (!string.IsNullOrEmpty(settings.Token))
                _opts.Token = settings.Token;

            _opts.DisconnectedEventHandler += (sender, args) =>
            {
                _logger.LogWarning("Connection to NATS disconnected. Trying to reconnect...");
                VerifyConnection();
            };
        }

        public IConnection GetConnection() => _connection;

        private bool IsConnected => _connection?.State == ConnState.CONNECTED && !_disposed;
        public void VerifyConnection()
        {
            if (IsConnected) return;
            
            var policy = Policy.Handle<NATSNoServersException>()
                .WaitAndRetryForever(
                    _ => TimeSpan.FromSeconds(2),
                    (ex, time) => _logger.LogWarning(ex.Message) );

            policy.Execute(() => {
                _connection = _factory.CreateConnection(_opts);
                if (IsConnected)
                {
                    _logger.LogInformation("Connected to NATS.");
                    Connected?.Invoke(_connection);
                } 
            });
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
