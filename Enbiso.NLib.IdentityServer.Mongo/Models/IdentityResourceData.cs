using Duende.IdentityServer.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Enbiso.NLib.IdentityServer.Mongo.Models
{
    public class IdentityResourceData
    {
        [BsonId]
        public string Id { get; set; }

        public IdentityResource Resource { get; set; }
    }
}