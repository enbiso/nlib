namespace Enbiso.NLib.IdentityServer.Mongo.Models;

public interface IMongoIdentityRole
{
    public IList<MongoClaim> Claims { get; set; }
}