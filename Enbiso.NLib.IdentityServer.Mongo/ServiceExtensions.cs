using Enbiso.NLib.IdentityServer.Mongo.Models;
using Enbiso.NLib.IdentityServer.Mongo.Stores;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIdentityServerMongo(this IServiceCollection services, Action<IdentityServerMongoOptions> optSetup)
        {
            var options = new IdentityServerMongoOptions();
            optSetup.Invoke(options);
            var mongo = new MongoClient(options.ConnectionString);
            var db = mongo.GetDatabase(options.Database);
            services.AddSingleton(sp => db.GetCollection<ClientData>(options.ClientCollection));
            services.AddSingleton(sp => db.GetCollection<PersistedGrantData>(options.PersistedGrantCollection));
            services.AddSingleton(sp => db.GetCollection<ApiResourceData>(options.ApiResourceCollection));
            services.AddSingleton(sp => db.GetCollection<IdentityResourceData>(options.IdentityResourceCollection));
            services.AddSingleton(sp => db.GetCollection<Role>(options.RoleCollection));
            services.AddSingleton(sp => db.GetCollection<User>(options.UserCollection));

            services.AddTransient<IClientStore, MongoClientStore>();
            services.AddTransient<IPersistedGrantStore, MongoPersistedGrantStore>();
            services.AddTransient<IResourceStore, MongoResourceStore>();
            services.AddTransient<IUserStore<User>, MongoUserStore>();
            services.AddTransient<IRoleStore<Role>, MongoRoleStore>();
            
            
            return services;
        }
    }

    public class IdentityServerMongoOptions
    {
        public string ConnectionString { get; set; } = "localhost";
        public string Database { get; set; } = "identity";
        public string ClientCollection { get; set; } = "clients";
        public string PersistedGrantCollection { get; set; } = "persisted-grants";
        public string ApiResourceCollection { get; set; } = "api-resources";
        public string IdentityResourceCollection { get; set; } = "identity-resources";
        public string RoleCollection { get; set; } = "roles";
        public string UserCollection { get; set; } = "users";
    }
}