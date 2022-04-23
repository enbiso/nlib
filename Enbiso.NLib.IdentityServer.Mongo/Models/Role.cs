using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Enbiso.NLib.IdentityServer.Mongo.Models
{
    public class Role : IdentityRole
    {
        public Role(string role) : base(role)
        {

        }

        public Role() : base()
        {

        }

        public IList<Claim> Claims = new List<Claim>();
    }
}