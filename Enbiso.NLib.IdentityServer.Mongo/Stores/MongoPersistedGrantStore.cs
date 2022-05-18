using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Enbiso.NLib.IdentityServer.Mongo.Models;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo.Stores
{
    public class MongoPersistedGrantStore: IPersistedGrantStore 
    {

        private readonly IMongoCollection<PersistedGrantData> _persistedGrants;

        public MongoPersistedGrantStore(IMongoCollection<PersistedGrantData> persistedGrants)
        {
            _persistedGrants = persistedGrants;
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            if(await _persistedGrants.Find(g => g.Id == grant.Key).AnyAsync())
                await RemoveAsync(grant.Key);
            
            await _persistedGrants.InsertOneAsync(new PersistedGrantData
            {
                Id = grant.Key,
                PersistedGrant = grant
            });
        }


        public Task<PersistedGrant> GetAsync(string key) => _persistedGrants.Find(p => p.Id == key)
            .Project(p => p.PersistedGrant).FirstOrDefaultAsync();

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var results = _persistedGrants
                .Find(p => 
                    p.PersistedGrant.Type == filter.Type && 
                    p.PersistedGrant.SessionId == filter.SessionId && 
                    p.PersistedGrant.SubjectId == filter.SubjectId && 
                    p.PersistedGrant.ClientId == filter.ClientId).Project(p => p.PersistedGrant).ToEnumerable();
            return Task.FromResult(results);
        }

        public Task RemoveAsync(string key) => _persistedGrants.DeleteOneAsync(w => w.Id == key);
        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            return _persistedGrants.DeleteManyAsync(p =>
                p.PersistedGrant.Type == filter.Type && 
                p.PersistedGrant.SessionId == filter.SessionId && 
                p.PersistedGrant.SubjectId == filter.SubjectId && 
                p.PersistedGrant.ClientId == filter.ClientId);
        }
    }
}