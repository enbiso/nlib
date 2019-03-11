using System;
using System.Data.Common;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Enbiso.NLib.EventLogger.Mongo
{
    public class MongoEventLog
    {
        [BsonId]
        public string Id { get; set; }
        public EventLog Log { get; set; }
    }
    
    public class MongoEventLogRepo : IEventLogRepo
    {        
        private readonly IMongoCollection<MongoEventLog> _logs;
        private readonly IMongoClient _client;
        public MongoEventLogRepo(IMongoClient client, string dbName)
        {
            _client = client;
            _logs = client.GetDatabase(dbName).GetCollection<MongoEventLog>("eventLogs");
        }

        public Task<EventLog> FindByIdAsync(Guid id) 
            => _logs.Find(l => l.Id == id.ToString()).FirstOrDefaultAsync().ContinueWith(t => t.Result?.Log);

        public EventLog Add(EventLog eventLog)
        {
            _logs.InsertOne(new MongoEventLog
            {
                Id = eventLog.ToString(),
                Log = eventLog
            });
            return eventLog;
        }

        public EventLog Update(EventLog eventLog) => _logs.FindOneAndUpdate(l => l.Id == eventLog.EventId.ToString(),
            Builders<MongoEventLog>.Update.Set(l => l.Log, eventLog)).Log;

        public void UseTransaction(DbTransaction transaction) {}

        public Task SaveChangesAsync() { return Task.CompletedTask; }
    }
}