using System;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;

namespace Enbiso.NLib.EventBus.AwsSns
{
    public class AwsSnsEventPublisher: IEventPublisher
    {
        private readonly IAwsSnsConnection _connection;
        private readonly AwsSnsOptions _options;

        public AwsSnsEventPublisher(IAwsSnsConnection connection, IOptions<AwsSnsOptions> options)
        {
            _connection = connection;
            _options = options.Value;
        }

        public async Task Publish<TEvent>(TEvent @event, string exchange, CancellationToken cancellationToken) where TEvent : IEvent
        {
            exchange ??= _options.PublishExchange;
            exchange = exchange?.Replace(".", "-");
            var topic = await _connection.GetTopic(exchange);
            
            var eventType = $"{@event.GetType().Name}";
            var message = JsonSerializer.Serialize(@event);

            var request = new PublishRequest(topic.TopicArn, message, eventType)
            {
                MessageAttributes =
                {
                    ["EventType"] = new MessageAttributeValue {DataType = "String", StringValue = eventType}
                }
            };

            await _connection.GetConnection().PublishAsync(request, cancellationToken);
        }
    }
}