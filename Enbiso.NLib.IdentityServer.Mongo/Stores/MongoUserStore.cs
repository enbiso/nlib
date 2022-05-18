using System.Security.Claims;
using Enbiso.NLib.IdentityServer.Mongo.Events;
using Enbiso.NLib.IdentityServer.Mongo.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo.Stores
{
    public class MongoUserStore<TUser> : 
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>, 
        IUserPasswordStore<TUser>, 
        IUserRoleStore<TUser>,
        IUserLoginStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserClaimStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,        
        IUserTwoFactorRecoveryCodeStore<TUser>,
        IProtectedUserStore<TUser> where TUser : IdentityUser, IMongoIdentityUser
    {
        
        private readonly IMongoCollection<TUser> _users;
        private readonly IIdentityServerMongoEventHandler<IdentityServerMongoUserCreateEvent<TUser>> _createEvent;
        private readonly IIdentityServerMongoEventHandler<IdentityServerMongoUserUpdateEvent<TUser>> _updateEvent;
        private readonly IIdentityServerMongoEventHandler<IdentityServerMongoUserDeleteEvent<TUser>> _deleteEvent;

        public MongoUserStore(IMongoCollection<TUser> users, 
            IIdentityServerMongoEventHandler<IdentityServerMongoUserCreateEvent<TUser>> createEvent, 
            IIdentityServerMongoEventHandler<IdentityServerMongoUserUpdateEvent<TUser>> updateEvent, 
            IIdentityServerMongoEventHandler<IdentityServerMongoUserDeleteEvent<TUser>> deleteEvent)
        {
            _users = users;
            _createEvent = createEvent;
            _updateEvent = updateEvent;
            _deleteEvent = deleteEvent;
        }

        public void Dispose()
        {            
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName);

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return _users.UpdateOneAsync(u => u.Id == user.Id, Builders<TUser>.Update.Set(u => u.UserName, userName),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.NormalizedUserName, normalizedName),
                cancellationToken: cancellationToken);
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                await _users.InsertOneAsync(user, cancellationToken:cancellationToken);
                await _createEvent.Handle(new IdentityServerMongoUserCreateEvent<TUser>(user), cancellationToken);
                return IdentityResult.Success;                
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = e.Message
                });
            }
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                await _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken:cancellationToken);
                await _updateEvent.Handle(new IdentityServerMongoUserUpdateEvent<TUser>(user), cancellationToken);
                return IdentityResult.Success;                
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = e.Message
                });
            }
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                await _users.DeleteOneAsync(u => u.Id == user.Id, cancellationToken);
                await _deleteEvent.Handle(new IdentityServerMongoUserDeleteEvent<TUser>(user), cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = e.Message
                });
            }
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            _users.Find(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            _users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync(cancellationToken);

        public IQueryable<TUser> Users => _users.AsQueryable();
        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.Email, email),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.EmailConfirmed);        

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.EmailConfirmed, confirmed),
                cancellationToken: cancellationToken);    
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
            _users.Find(u => u.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync(cancellationToken);

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedEmail);

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.NormalizedEmail, normalizedEmail),
                cancellationToken: cancellationToken);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.PasswordHash, passwordHash),
                cancellationToken: cancellationToken);      
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            user.Roles ??= new List<string>();
            if (user.Roles.Contains(roleName)) return Task.CompletedTask;
            user.Roles.Add(roleName);
            return _users.UpdateOneAsync(u => u.Id == user.Id, Builders<TUser>.Update.Push(u => u.Roles, roleName),
                cancellationToken: cancellationToken);
        }


        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            user.Roles ??= new List<string>();
            if (!user.Roles.Contains(roleName)) return Task.CompletedTask;
            user.Roles.Remove(roleName);
            return _users.UpdateOneAsync(u => u.Id == user.Id, Builders<TUser>.Update.Pull(u => u.Roles, roleName),
                cancellationToken: cancellationToken);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Roles);

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
            => Task.FromResult(user.Roles.Contains(roleName));

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var users = await _users.Find(u => u.Roles.Contains(roleName))
                .ToListAsync(cancellationToken);
            return users;
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.Logins.Add(login);
            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            bool Filter(UserLoginInfo l) => !(l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
            user.Logins = user.Logins.Where(Filter).ToList();
            return Task.CompletedTask;
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            var logins = user.Logins ?? new List<UserLoginInfo>();
            return Task.FromResult(logins);
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            return _users.Find(u => 
                    u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.LockoutEnd);

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.LockoutEnd, lockoutEnd),
                cancellationToken: cancellationToken);
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            await _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.AccessFailedCount, user.AccessFailedCount),
                cancellationToken: cancellationToken);
            return user.AccessFailedCount;
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.AccessFailedCount, 0),
                cancellationToken: cancellationToken);            
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.AccessFailedCount);

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.LockoutEnabled);

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;            
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.LockoutEnabled, enabled),
                cancellationToken: cancellationToken);            
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Claims);
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.Claims ??= new List<Claim>();
            foreach (var claim in claims) 
                user.Claims.Add(claim);

            return _users.UpdateOneAsync(u => u.Id == user.Id,
            Builders<TUser>.Update.PushEach(u => u.Claims, user.Claims),
            cancellationToken: cancellationToken);
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            user.Claims ??= new List<Claim>();
            var uClaim = user.Claims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);
            user.Claims.Remove(uClaim);
            user.Claims.Add(newClaim);
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Pull(u => u.Claims, uClaim).Push(u => u.Claims, newClaim),
                cancellationToken: cancellationToken);
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            claims = claims == null ? new List<Claim>() : claims.ToList();
            foreach (var claim in claims) 
                user.Claims.Remove(claim);

            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.PullAll(u => u.Claims, claims),
                cancellationToken: cancellationToken);
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return await _users.Find(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                .ToListAsync(cancellationToken);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.TwoFactorEnabled, enabled),
                cancellationToken: cancellationToken);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.TwoFactorEnabled);

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.PhoneNumber, phoneNumber),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PhoneNumber);

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PhoneNumberConfirmed);

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.PhoneNumberConfirmed, confirmed),
                cancellationToken: cancellationToken);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.SecurityStamp, stamp),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.SecurityStamp);

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            user.AuthenticatorKey = key;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.AuthenticatorKey, key),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.AuthenticatorKey);

        private static string TokenKey(string loginProvider, string name) => $"{loginProvider}.{name}";
        
        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            var key = TokenKey(loginProvider, name);
            user.Tokens[key] = value;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Push(u => u.Tokens, new KeyValuePair<string, string>(key, value)),
                cancellationToken: cancellationToken);
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var key = TokenKey(loginProvider, name);
            user.Tokens.Remove(key);
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.PullFilter(u => u.Tokens, t => t.Key == key),
                cancellationToken: cancellationToken);   
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name,
            CancellationToken cancellationToken) => Task.FromResult(user.Tokens[TokenKey(loginProvider, name)]);

        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            user.RecoveryCodes = recoveryCodes?.Select(r => new RecoveryCode {Code = r, Used = false}).ToList();
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.RecoveryCodes, user.RecoveryCodes),
                cancellationToken: cancellationToken);
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            var recoveryCode = user.RecoveryCodes.FirstOrDefault(c => c.Code == code && !c.Used);
            if (recoveryCode == null) return false;

            user.RecoveryCodes[user.RecoveryCodes.IndexOf(recoveryCode)].Used = true;
            
            await _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<TUser>.Update.Set(u => u.RecoveryCodes, user.RecoveryCodes),
                cancellationToken: cancellationToken);
            return true;
        }

        public Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.RecoveryCodes.Count(c => !c.Used));
    }
}