using System.Security.Claims;
using Enbiso.NLib.IdentityServer.Mongo.Events;
using Enbiso.NLib.IdentityServer.Mongo.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo.Stores
{
    public class MongoUserStore : 
        IQueryableUserStore<User>,
        IUserEmailStore<User>, 
        IUserPasswordStore<User>, 
        IUserRoleStore<User>,
        IUserLoginStore<User>,
        IUserLockoutStore<User>,
        IUserClaimStore<User>,
        IUserTwoFactorStore<User>,
        IUserPhoneNumberStore<User>,
        IUserSecurityStampStore<User>,
        IUserAuthenticatorKeyStore<User>,
        IUserAuthenticationTokenStore<User>,        
        IUserTwoFactorRecoveryCodeStore<User>,
        IProtectedUserStore<User>
    {
        
        private readonly IMongoCollection<User> _users;
        private readonly IIdentityServerMongoEventHandler<IdentityServerMongoUserCreateEvent> _createEvent;
        private readonly IIdentityServerMongoEventHandler<IdentityServerMongoUserUpdateEvent> _updateEvent;
        private readonly IIdentityServerMongoEventHandler<IdentityServerMongoUserDeleteEvent> _deleteEvent;

        public MongoUserStore(IMongoCollection<User> users, 
            IIdentityServerMongoEventHandler<IdentityServerMongoUserCreateEvent> createEvent, 
            IIdentityServerMongoEventHandler<IdentityServerMongoUserUpdateEvent> updateEvent, 
            IIdentityServerMongoEventHandler<IdentityServerMongoUserDeleteEvent> deleteEvent)
        {
            _users = users;
            _createEvent = createEvent;
            _updateEvent = updateEvent;
            _deleteEvent = deleteEvent;
        }

        public void Dispose()
        {            
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName);

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return _users.UpdateOneAsync(u => u.Id == user.Id, Builders<User>.Update.Set(u => u.UserName, userName),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.NormalizedUserName, normalizedName),
                cancellationToken: cancellationToken);
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _users.InsertOneAsync(user, cancellationToken:cancellationToken);
                await _createEvent.Handle(new IdentityServerMongoUserCreateEvent(user), cancellationToken);
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

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken:cancellationToken);
                await _updateEvent.Handle(new IdentityServerMongoUserUpdateEvent(user), cancellationToken);
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

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _users.DeleteOneAsync(u => u.Id == user.Id, cancellationToken);
                await _deleteEvent.Handle(new IdentityServerMongoUserDeleteEvent(user), cancellationToken);
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

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            _users.Find(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            _users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync(cancellationToken);

        public IQueryable<User> Users => _users.AsQueryable();
        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.Email, email),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.EmailConfirmed);        

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.EmailConfirmed, confirmed),
                cancellationToken: cancellationToken);    
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
            _users.Find(u => u.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync(cancellationToken);

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedEmail);

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.NormalizedEmail, normalizedEmail),
                cancellationToken: cancellationToken);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.PasswordHash, passwordHash),
                cancellationToken: cancellationToken);      
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user.Roles.Contains(roleName)) return Task.CompletedTask;
            user.Roles.Add(roleName);
            return _users.UpdateOneAsync(u => u.Id == user.Id, Builders<User>.Update.Push(u => u.Roles, roleName),
                cancellationToken: cancellationToken);
        }


        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (!user.Roles.Contains(roleName)) return Task.CompletedTask;
            user.Roles.Remove(roleName);
            return _users.UpdateOneAsync(u => u.Id == user.Id, Builders<User>.Update.Pull(u => u.Roles, roleName),
                cancellationToken: cancellationToken);
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.Roles);

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
            => Task.FromResult(user.Roles.Contains(roleName));

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var users = await _users.Find(u => u.Roles.Contains(roleName))
                .ToListAsync(cancellationToken);
            return users;
        }

        public Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (login is ExternalLoginInfo info)
                user.ExternalLogins.Add(info);    
            else
                user.Logins.Add(login);
            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            bool Filter(UserLoginInfo l) => !(l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
            user.Logins = user.Logins.Where(Filter).ToList();
            user.ExternalLogins = user.ExternalLogins.Where(Filter).ToList();
            return Task.CompletedTask;
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            var logins = user.Logins ?? new List<UserLoginInfo>();
            foreach (var externalLogin in user.ExternalLogins)
                logins.Add(externalLogin);
            return Task.FromResult(logins);
        }

        public Task<User> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            return _users.Find(u => 
                    u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey) || 
                    u.ExternalLogins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.LockoutEnd);

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.LockoutEnd, lockoutEnd),
                cancellationToken: cancellationToken);
        }

        public async Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            await _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.AccessFailedCount, user.AccessFailedCount),
                cancellationToken: cancellationToken);
            return user.AccessFailedCount;
        }

        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.AccessFailedCount, 0),
                cancellationToken: cancellationToken);            
        }

        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.AccessFailedCount);

        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.LockoutEnabled);

        public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;            
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.LockoutEnabled, enabled),
                cancellationToken: cancellationToken);            
        }

        public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Claims);
        }

        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims) 
                user.Claims.Add(claim);

            return _users.UpdateOneAsync(u => u.Id == user.Id,
            Builders<User>.Update.PushEach(u => u.Claims, user.Claims),
            cancellationToken: cancellationToken);
        }

        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            var uClaim = user.Claims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);
            user.Claims.Remove(uClaim);
            user.Claims.Add(newClaim);
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Pull(u => u.Claims, uClaim).Push(u => u.Claims, newClaim),
                cancellationToken: cancellationToken);
        }

        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            claims = claims == null ? new List<Claim>() : claims.ToList();
            foreach (var claim in claims) 
                user.Claims.Remove(claim);

            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.PullAll(u => u.Claims, claims),
                cancellationToken: cancellationToken);
        }

        public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return await _users.Find(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                .ToListAsync(cancellationToken);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.TwoFactorEnabled, enabled),
                cancellationToken: cancellationToken);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.TwoFactorEnabled);

        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.PhoneNumber, phoneNumber),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PhoneNumber);

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PhoneNumberConfirmed);

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.PhoneNumberConfirmed, confirmed),
                cancellationToken: cancellationToken);
        }

        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.SecurityStamp, stamp),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.SecurityStamp);

        public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
        {
            user.AuthenticatorKey = key;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.AuthenticatorKey, key),
                cancellationToken: cancellationToken);
        }

        public Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.AuthenticatorKey);

        private static string TokenKey(string loginProvider, string name) => $"{loginProvider}.{name}";
        
        public Task SetTokenAsync(User user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            var key = TokenKey(loginProvider, name);
            user.Tokens[key] = value;
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Push(u => u.Tokens, new KeyValuePair<string, string>(key, value)),
                cancellationToken: cancellationToken);
        }

        public Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var key = TokenKey(loginProvider, name);
            user.Tokens.Remove(key);
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.PullFilter(u => u.Tokens, t => t.Key == key),
                cancellationToken: cancellationToken);   
        }

        public Task<string> GetTokenAsync(User user, string loginProvider, string name,
            CancellationToken cancellationToken) => Task.FromResult(user.Tokens[TokenKey(loginProvider, name)]);

        public Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            user.RecoveryCodes = recoveryCodes?.Select(r => new RecoveryCode {Code = r, Used = false}).ToList();
            return _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.RecoveryCodes, user.RecoveryCodes),
                cancellationToken: cancellationToken);
        }

        public async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
        {
            var recoveryCode = user.RecoveryCodes.FirstOrDefault(c => c.Code == code && !c.Used);
            if (recoveryCode == null) return false;

            user.RecoveryCodes[user.RecoveryCodes.IndexOf(recoveryCode)].Used = true;
            
            await _users.UpdateOneAsync(u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.RecoveryCodes, user.RecoveryCodes),
                cancellationToken: cancellationToken);
            return true;
        }

        public Task<int> CountCodesAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.RecoveryCodes.Count(c => !c.Used));
    }
}