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
        private readonly ILogger _logger;
        private readonly RabbitMqOption _options;

        public RabbitMqBusPublisher(IRabbitMqConnection connection, IOptions<RabbitMqOption> options, ILogger<RabbitMqBusPublisher> logger)
        {
            _connection = connection;
            _logger = logger;
            _options = options.Value;
        }

        public Task Publish<TEvent>(TEvent @event, string exchange, CancellationToken cancellationToken) where TEvent : IEvent
        {
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_options.PublishRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, _) =>
                    {
                        _logger.LogWarning(ex.ToString());
                    });

            _connection.VerifyConnection();
            
            exchange ??= _options.PublishExchange ?? _options.Exchanges.FirstOrDefault();
            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchange, type: "direct");

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            
            var eventName = @event.GetType().Name;

            policy.Execute(() =>
            {
                channel.BasicPublish(exchange, eventName, null, body);
            });
                
            return Task.CompletedTask;
        }
    }
}