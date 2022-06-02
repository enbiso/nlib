using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;
using Polly;
using Options = NATS.Client.Options;

namespace Enbiso.NLib.EventBus.Nats
{
    public interface INatsConnection : IDisposable
    {
        IConnection GetConnection();
        IJetStream GetJetStream();
        void VerifyConnection();
        event ConnectedEventHandler Connected;
        void VerifyJetStream(string streamName);
    }

    public delegate void ConnectedEventHandler(IConnection connection);

    public class NatsConnection: INatsConnection
    {
        private IConnection _connection;
        private IJetStream _jetStream;
        private IJetStreamManagement _jsm;
        private readonly NatsOptions _settings;
        private readonly ConnectionFactory _factory;
        private readonly Options _opts;
        private readonly ILogger _logger;
        private bool _disposed;
        public event ConnectedEventHandler Connected;

        public NatsConnection(ConnectionFactory factory, IOptions<NatsOptions> options, ILogger<NatsConnection> logger)
        {
            _logger = logger;
            _factory = factory;
            
            _settings = options.Value;
            _opts = ConnectionFactory.GetDefaultOptions();
            _opts.Servers = _settings.Servers;
            
            _opts.MaxReconnect = _settings.RetryCount;
            if (!string.IsNullOrEmpty(_settings.Username))
                _opts.User = _settings.Username;
            if (!string.IsNullOrEmpty(_settings.Password))
                _opts.Password = _settings.Password;
            if (!string.IsNullOrEmpty(_settings.Token))
                _opts.Token = _settings.Token;

            _opts.DisconnectedEventHandler += (sender, args) =>
            {
                _logger.LogWarning("Connection to NATS disconnected. Trying to reconnect...");
                VerifyConnection();
            };
        }
        
        public void VerifyJetStream(string streamName)
        {
            var streamNames = _jsm.GetStreamNames();
            if (streamNames.Contains(streamName)) return;
            
            var sc = StreamConfiguration.Builder()
                .WithName(streamName)
                .WithStorageType(StorageType.Memory)
                .WithRetentionPolicy(RetentionPolicy.Interest) //wait for all consumers
                .WithSubjects($"{streamName}.>")
                .Build();
            _jsm.AddStream(sc);
        }

        public IConnection GetConnection() => _connection;
        public IJetStream GetJetStream() => _jetStream;
        private bool IsConnected => _connection?.State == ConnState.CONNECTED && !_disposed;
        public void VerifyConnection()
        {
            if (IsConnected) return;
            
            var policy = Policy.Handle<NATSNoServersException>()
                .WaitAndRetryForever(
                    _ => TimeSpan.FromSeconds(2),
                    (ex, _) => _logger.LogWarning(ex.Message) );

            policy.Execute(() => {
                _connection = _factory.CreateConnection(_opts);
                if (IsConnected)
                {
                    _logger.LogInformation("Connected to NATS");
                    if (_settings.JetStreamEnable)
                    {
                        _jetStream = _connection.CreateJetStreamContext();
                        _jsm = _connection.CreateJetStreamManagementContext();
                        _logger.LogInformation("JetStream Initialised");
                    }
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
                _connection.Drain();
                _connection.Close();
                _connection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

    }
}
