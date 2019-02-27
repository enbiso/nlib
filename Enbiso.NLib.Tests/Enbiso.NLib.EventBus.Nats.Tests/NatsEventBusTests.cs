using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NATS.Client;
using NSubstitute;
using Xunit;
using Options = Microsoft.Extensions.Options.Options;

namespace Enbiso.NLib.EventBus.Nats.Tests
{
    public class NatsEventBusTests
    {
        [Fact]
        public void SamePublishSubscribeTests()
        {
            var opts = Options.Create(new NatsOptions
            {
                Servers = new [] { "nats://localhost:4222" },
                Exchange = "testEx",
                Client = "tClient"
            });
            var pConnLogger = new Logger<NatsPersistentConnection>(new NullLoggerFactory());
            var pConn = new NatsPersistentConnection(new ConnectionFactory(), opts, pConnLogger);
            var eventProcessor = Substitute.For<IEventProcessor>();            
            var busLogger = new Logger<NatsEventBus>(new NullLoggerFactory());
            var bus = new NatsEventBus(opts, pConn, eventProcessor, new EventBusSubscriptionsManager(), busLogger);            
            bus.Initialize();            
            bus.Publish(new TestEvent());
            Thread.Sleep(1000);
            eventProcessor.Received().ProcessEvent(typeof(TestEvent).Name, Arg.Any<byte[]>());
        }

        [Fact]
        public void SeperatePublishSubscribeTests()
        {
            var opts1 = Options.Create(new NatsOptions
            {
                Servers = new[] { "nats://localhost:4222" },
                Exchange = "testEx",
                Client = "tClient1"
            });

            var opts2 = Options.Create(new NatsOptions
            {
                Servers = new[] { "nats://localhost:4222" },
                Exchange = "testEx",
                Client = "tClient2"
            });

            var pConnLogger = new Logger<NatsPersistentConnection>(new NullLoggerFactory());
            var pConn = new NatsPersistentConnection(new ConnectionFactory(), opts1, pConnLogger);
            var eventProcessor1 = Substitute.For<IEventProcessor>();
            var eventProcessor2 = Substitute.For<IEventProcessor>();
            var busLogger = new Logger<NatsEventBus>(new NullLoggerFactory());
            var bus1 = new NatsEventBus(opts1, pConn, eventProcessor1, new EventBusSubscriptionsManager(), busLogger);
            var bus2 = new NatsEventBus(opts2, pConn, eventProcessor2, new EventBusSubscriptionsManager(), busLogger);
            bus2.Subscribe<TestEvent, TestEventHandler>();
            bus1.Initialize();
            bus2.Initialize();
            bus1.Publish(new TestEvent());
            Thread.Sleep(1000);
            eventProcessor2.Received().ProcessEvent(typeof(TestEvent).Name, Arg.Any<byte[]>());
            eventProcessor1.DidNotReceive().ProcessEvent(typeof(TestEvent).Name, Arg.Any<byte[]>());
        }
    }

    internal class TestEvent : IEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreationDate { get; } = DateTime.Now;
    }

    internal class TestEventHandler: IEventHandler<TestEvent>
    {
        public Task Handle(TestEvent @event) => Task.CompletedTask;        
    }
}
