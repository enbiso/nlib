using System.Collections.Generic;
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
        private readonly IDictionary<string, Topic> _topics;

        public AwsSnsConnection(IAmazonSimpleNotificationService connection)
        {
            _connection = connection;
            _topics = new Dictionary<string, Topic>();
        }

        public IAmazonSimpleNotificationService GetConnection() => _connection;

        public async Task<Topic> GetTopic(string exchangeName)
        {
            if (_topics.TryGetValue(exchangeName, out var existingTopic))
                return existingTopic;
            var topic = await _connection.FindTopicAsync(exchangeName);
            _topics.Add(exchangeName, topic);
            return topic;
        }
    }
}
