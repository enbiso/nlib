using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace Enbiso.NLib.EventBus.AwsSns
{
    public interface IAwsSnsConnection
    {
        IAmazonSimpleNotificationService GetConnection();
        Task<Topic> GetTopic(string exchangeName);
    }

    public class AwsSnsConnection: IAwsSnsConnection
    {
        private readonly IAmazonSimpleNotificationService _connection;
        private readonly Dictionary<string, Topic> _topics = new();

        public AwsSnsConnection(IAmazonSimpleNotificationService connection)
        {
            _connection = connection;
        }

        public IAmazonSimpleNotificationService GetConnection() => _connection;

        public async Task<Topic> GetTopic(string exchangeName)
        {
            if (_topics.TryGetValue(exchangeName, out var existingTopic))
                return existingTopic;
            var topic = await _connection.FindTopicAsync(exchangeName);
            if (topic == null)
            {
                await _connection.CreateTopicAsync(exchangeName);
                topic = await _connection.FindTopicAsync(exchangeName);
            }
            _topics[exchangeName] = topic;
            return topic;
        }
    }
}
