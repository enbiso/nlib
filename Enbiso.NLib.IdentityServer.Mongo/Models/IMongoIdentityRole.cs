using System.Security.Claims;

namespace Enbiso.NLib.IdentityServer.Mongo.Models;

public interface IMongoIdentityRole
{
    public IList<Claim> Claims { get; set; }
}