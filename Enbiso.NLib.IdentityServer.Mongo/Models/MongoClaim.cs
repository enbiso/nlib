using System.Security.Claims;
using MongoDB.Bson.Serialization.Attributes;

namespace Enbiso.NLib.IdentityServer.Mongo.Models;

[BsonIgnoreExtraElements]
public class MongoClaim
{
    public MongoClaim()
    {
        
    }
    public MongoClaim(Claim claim)
    {
        Type = claim.Type;
        Value = claim.Value;
        ValueType = claim.ValueType;
        Issuer = claim.Issuer;
        OriginalIssuer = claim.OriginalIssuer;
    }
    
    public Claim ToClaim() => new(Type, Value, ValueType, Issuer, OriginalIssuer);
    public string Type { get; set; }
    public string OriginalIssuer { get; set; }
    public string Issuer { get; set; }
    public string ValueType { get; set; }
    public string Value { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as MongoClaim);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, OriginalIssuer, Issuer, ValueType, Value);
    }

    private bool Equals ( MongoClaim obj )
    {
        return obj != null && obj.Type == Type && obj.Value == Value;
    }
}