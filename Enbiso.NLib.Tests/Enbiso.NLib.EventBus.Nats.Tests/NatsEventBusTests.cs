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
                Exchanges = new [] {"testEx"},
                Client = "tClient"
            });
            var pConnLogger = new Logger<NatsConnection>(new NullLoggerFactory());
            var pConn = new NatsConnection(new ConnectionFactory(), opts, pConnLogger);
            var eventProcessor = Substitute.For<IEventProcessor>();            
            var busLogger = new Logger<NatsEventPublisher>(new NullLoggerFactory());
            var bus = new NatsEventPublisher(pConn, opts, busLogger);
            bus.Publish(new TestEvent(), null, CancellationToken.None);
            Thread.Sleep(1000);
            eventProcessor.Received().ProcessEvent(typeof(TestEvent).Name, Arg.Any<byte[]>());
        }
    }

    internal class TestEvent : IEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime EventCreationDate { get; } = DateTime.Now;
    }

    internal class TestEventHandler: EventHandler<TestEvent>
    {
        protected override Task Handle(TestEvent @event) => Task.CompletedTask;        
    }
}
