using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Enbiso.NLib.IdentityServer.Mongo.Models;

public interface IMongoIdentityUser
{
    IList<string> Roles { get; set; }
    IList<UserLoginInfo> Logins { get; set; }
    IList<ExternalLoginInfo> ExternalLogins { get; set; }
    IList<Claim> Claims { get; set; }
    string AuthenticatorKey { get; set; }
    IDictionary<string, string> Tokens { get; set; }
    IList<RecoveryCode> RecoveryCodes { get; set; }
}