using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace Enbiso.NLib.IdentityServer.Mongo.Models;

[BsonIgnoreExtraElements]
public class MongoUserLoginInfo : UserLoginInfo
{
    public MongoUserLoginInfo(string loginProvider, string providerKey, string displayName) : base(loginProvider,
        providerKey, displayName)
    {
    }
}