using Duende.IdentityServer.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Enbiso.NLib.IdentityServer.Mongo.Models
{
    public class ClientData
    {
        [BsonId]
        public string Id { get; set; }
        public Client Client { get; set; }
    }
}