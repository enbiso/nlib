namespace Enbiso.NLib.IdentityServer.Mongo.Models;

public interface IMongoIdentityUser
{
    IList<string> Roles { get; set; }
    IList<MongoUserLoginInfo> Logins { get; set; }
    IList<MongoClaim> Claims { get; set; }
    string AuthenticatorKey { get; set; }
    IDictionary<string, string> Tokens { get; set; }
    IList<RecoveryCode> RecoveryCodes { get; set; }
}