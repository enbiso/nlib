using IdentityServer4.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Enbiso.NLib.IdentityServer.Mongo.Models
{
    public class PersistedGrantData
    {
        [BsonId]
        public string Id { get; set; }

        public PersistedGrant PersistedGrant { get; set; }
    }
}