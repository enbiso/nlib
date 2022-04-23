using Enbiso.NLib.IdentityServer.Mongo.Models;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo.Stores
{
    public class MongoClientStore: IClientStore
    {
        private readonly IMongoCollection<ClientData> _clients;

        public MongoClientStore(IMongoCollection<ClientData> clients)
        {
            _clients = clients;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var wrapper = await _clients.Find(c => c.Id == clientId).FirstOrDefaultAsync();
            return wrapper?.Client;
        }       
    }
}