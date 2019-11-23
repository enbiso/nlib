using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    public class RabbitMqEventSubscriber: IEventSubscriber, IDisposable
    {
        private IModel _consumerChannel;
        private readonly IRabbitMqConnection _connection;
        private readonly IEventProcessor _eventProcessor;
        private readonly string _queueName;
        private readonly IEnumerable<string> _exchanges;

        public RabbitMqEventSubscriber(IRabbitMqConnection connection, IEventProcessor eventProcessor, IOptions<RabbitMqOption> optionWrap)
        {
            var option = optionWrap.Value;
            _queueName = option.Client;
            _exchanges = option.Exchanges;

            _connection = connection;
            _eventProcessor = eventProcessor;
            _consumerChannel = CreateConsumerChannel();
        }
        
        public void Initialize()
        {
            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;                
                await _eventProcessor.ProcessEvent(eventName, ea.Body);
                // ACK
                _consumerChannel.BasicAck(ea.DeliveryTag, multiple:false);
            };
            _consumerChannel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }

        public Task Subscribe(string eventName, CancellationToken cancellationToken = default)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                foreach (var exchange in _exchanges)
                {
                    channel.QueueBind(queue: _queueName, exchange: exchange, routingKey: eventName);
                }
            }
            return Task.CompletedTask;
        }

        public Task UnSubscribe(string eventName, CancellationToken cancellationToken = default)
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            using var channel = _connection.CreateModel();
            foreach (var exchange in _exchanges)
            {
                channel.QueueUnbind(_queueName, exchange, eventName);
            }
            return Task.CompletedTask;
        }
        
        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            var channel = _connection.CreateModel();

            foreach (var exchange in _exchanges)
            {
                channel.ExchangeDeclare(exchange: exchange, type: "direct");
            }

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }
        
        public void Dispose()
        {
            _consumerChannel?.Dispose();
        }
    }
}