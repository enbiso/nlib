using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Enbiso.NLib.IdentityServer.Mongo.Models;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo.Stores
{
    public class MongoResourceStore : IResourceStore
    {
        private readonly IMongoCollection<ApiResourceData> _apiResources;
        private readonly IMongoCollection<IdentityResourceData> _identityResources;

        public MongoResourceStore(IMongoCollection<IdentityResourceData> identityResources,
            IMongoCollection<ApiResourceData> apiResources)
        {
            _identityResources = identityResources;
            _apiResources = apiResources;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var wrappers = await _identityResources
                .Find(Builders<IdentityResourceData>.Filter.In(w => w.Id, scopeNames)).ToListAsync();                
            return wrappers.Select(w => w.Resource);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return await _apiResources.Find(Builders<ApiResourceData>.Filter.AnyIn(a => a.Resource.Scopes, scopeNames))
                .Project(w => w.Resource)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var resources = await FindApiResourcesByScopeNameAsync(scopeNames);
            resources ??= new List<ApiResource>();
            return resources.SelectMany(r => r.Scopes)
                .Distinct()
                .Select(s => new ApiScope(s));
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var resources = await _apiResources
                .Find(Builders<ApiResourceData>.Filter.In(a => a.Resource.Name, apiResourceNames))
                .Project(w => w.Resource)
                .ToListAsync();
            return resources;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiResTask = _apiResources.Find(_ => true).Project(a => a.Resource).ToListAsync();
            var idResTask = _identityResources.Find(_ => true).Project(a => a.Resource).ToListAsync();
            await Task.WhenAll(apiResTask, idResTask);
            var apiResources = await apiResTask;
            var idResources = await idResTask;
            var apiScopes = apiResources.SelectMany(r => r.Scopes)
                .Distinct()
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => new ApiScope(s));
            return new Resources(idResources, apiResources, apiScopes);
        }
    }
}