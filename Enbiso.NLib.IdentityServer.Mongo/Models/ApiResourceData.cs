using Duende.IdentityServer.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Enbiso.NLib.IdentityServer.Mongo.Models
{
    public class ApiResourceData
    {
        [BsonId]
        public string Id { get; set; }

        public ApiResource Resource { get; set; }
    }
}