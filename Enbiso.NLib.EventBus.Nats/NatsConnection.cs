using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
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

            var policy = NatsPolicyBuilder.BuildConnectPolicy(_logger);

            policy.Execute(() =>
            {
                var servers = _opts.Servers ?? Array.Empty<string>();
                _logger.LogInformation("Connecting to {Servers}", string.Join(", ", servers));
                _connection = _factory.CreateConnection(_opts);
                if (!IsConnected) return;
                _logger.LogInformation("Connected to NATS");
                Connected?.Invoke(_connection);
            });
        }
        

 
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try
            {
                _connection.Drain();
                _connection.Close();
                _connection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("{Error}",ex.ToString());
            }
        }

    }
}
