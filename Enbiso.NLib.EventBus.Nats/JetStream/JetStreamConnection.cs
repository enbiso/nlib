using System;
using Microsoft.Extensions.Logging;
using NATS.Client.JetStream;

namespace Enbiso.NLib.EventBus.Nats.JetStream;

public interface IJetStreamConnection: IDisposable
{
    void VerifyJetStream(string streamName);
    public IJetStream GetJetStream();
    
    event ConnectedEventHandler Connected;
}

public class JetStreamConnection: IJetStreamConnection
{
    private IJetStreamManagement _jetStreamManagement;
    private IJetStream _jetStream;
    private readonly INatsConnection _connection;
    
    public event ConnectedEventHandler Connected;
    
    public JetStreamConnection(INatsConnection connection, ILogger<JetStreamConnection> logger)
    {
        _connection = connection;
        connection.Connected += conn =>
        {
            _jetStreamManagement = conn.CreateJetStreamManagementContext();
            _jetStream = conn.CreateJetStreamContext();
            logger.LogInformation("JetStream Initialised");
            
            Connected?.Invoke(conn);
        };
    }

    public void VerifyJetStream(string streamName)
    {
        _connection.VerifyConnection();
        
        var streamNames = _jetStreamManagement.GetStreamNames();
        if (streamNames.Contains(streamName)) return;
            
        var sc = StreamConfiguration.Builder()
            .WithName(streamName)
            .WithStorageType(StorageType.Memory)
            .WithRetentionPolicy(RetentionPolicy.Interest) //wait for all consumers
            .WithSubjects($"{streamName}.>")
            .Build();
        _jetStreamManagement.AddStream(sc);
    }
    
    public IJetStream GetJetStream() => _jetStream;


    public void Dispose()
    {
        _connection?.Dispose();
    }
}