using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Enbiso.NLib.IdentityServer.Mongo.Models
{
    public class User : IdentityUser
    {
        public User(string userName) : base(userName)
        {
        }
        public User() : base()
        {
        }
        public IList<string> Roles { get; set; } = new List<string>();
        public IList<UserLoginInfo> Logins { get; set; } = new List<UserLoginInfo>();
        public IList<ExternalLoginInfo> ExternalLogins { get; set; } = new List<ExternalLoginInfo>();
        public IList<Claim> Claims { get; set; } = new List<Claim>();
        public string AuthenticatorKey { get; set; }
        public IDictionary<string, string> Tokens { get; set; } = new Dictionary<string, string>();
        public IList<RecoveryCode> RecoveryCodes { get; set; } = new List<RecoveryCode>();
    }

    public class RecoveryCode
    {
        public string Code { get; set; }
        public bool Used { get; set; }
    }
}