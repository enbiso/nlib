using System;
using System.Collections.Generic;
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
using RabbitMQ.Client.Exceptions;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    public class RabbitMqBusPublisher: IEventPublisher
    {
        private readonly IRabbitMqConnection _connection;
        private readonly int _retryCount;
        private readonly IEnumerable<string> _exchanges;
        private readonly ILogger _logger;

        public RabbitMqBusPublisher(IRabbitMqConnection connection, IOptions<RabbitMqOption> optionWrap, ILogger<RabbitMqBusPublisher> logger)
        {
            var option = optionWrap.Value;
            _exchanges = option.Exchanges;
            _retryCount = option.RetryCount;

            _connection = connection;
            _logger = logger;
        }

        public Task Publish<TEvent>(TEvent @event, string exchange, CancellationToken cancellationToken) where TEvent : IEvent
        {
            _connection.VerifyConnection();

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