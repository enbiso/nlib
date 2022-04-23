using System.Security.Claims;
using Enbiso.NLib.IdentityServer.Mongo.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo.Stores
{
    public class MongoRoleStore<TRole> : IQueryableRoleStore<TRole>, IRoleClaimStore<TRole> where TRole : IdentityRole, IMongoIdentityRole
    {
        private readonly IMongoCollection<TRole> _roles;

        public MongoRoleStore(IMongoCollection<TRole> roles)
        {
            _roles = roles;
        }

        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                await _roles.InsertOneAsync(role, cancellationToken: cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = e.Message,
                });
            }
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                await _roles.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = e.Message,
                });
            }
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                await _roles.DeleteOneAsync(r => r.Id == role.Id, cancellationToken: cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = e.Message,
                });
            }
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) =>
            Task.FromResult(role.Id);

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) =>
            Task.FromResult(role.Name);

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) =>
            Task.FromResult(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken token) =>
            _roles.Find(r => r.Id == roleId).FirstOrDefaultAsync(token);

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken token) =>
            _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(token);

        public IQueryable<TRole> Roles => _roles.AsQueryable();

        public Task<IList<Claim>> GetClaimsAsync(TRole role,
            CancellationToken cancellationToken = new CancellationToken())
        {
            IList<Claim> claims = role.Claims.ToList();
            return Task.FromResult(claims);
        }

        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            role.Claims.Add(claim);
            return _roles.UpdateOneAsync(r => r.Id == role.Id, Builders<TRole>.Update.Push(r => r.Claims, claim),
                cancellationToken: cancellationToken);
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            var mClaim = role.Claims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);
            role.Claims.Remove(mClaim);
            return _roles.UpdateOneAsync(r => r.Id == role.Id, Builders<TRole>.Update.Pull(r => r.Claims, mClaim),
                cancellationToken: cancellationToken);
        }
    }
}