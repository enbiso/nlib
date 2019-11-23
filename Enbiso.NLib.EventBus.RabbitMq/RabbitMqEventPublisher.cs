using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    /// <inheritdoc />
    /// <summary>
    /// Rabbit implementation of @event bus
    /// </summary>
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly string[] _exchanges;        

        private readonly IRabbitMqConnection _connection;
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private readonly int _retryCount;

        public RabbitMqEventPublisher(
            IRabbitMqConnection connection, 
            ILogger<RabbitMqEventPublisher> logger,
            IOptions<RabbitMqOption> optionWrap)
        {
            var option = optionWrap.Value;
            _exchanges = option.Exchanges ?? throw new ArgumentNullException(nameof(option.Exchanges));            
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = option.PublishRetryCount;
        }

        public Task Publish<T>(T @event, string exchange = null, CancellationToken cancellationToken = default) where T:IEvent
        {            
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()                
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchange, type: "direct");

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var eventName = @event.GetType().Name;
            exchange ??= _exchanges.FirstOrDefault();
            
            policy.Execute(() =>
            {
                channel.BasicPublish(exchange: exchange, routingKey: eventName, basicProperties: null, body: body);
            });
                
            return Task.CompletedTask;
        }

       
    }
}

