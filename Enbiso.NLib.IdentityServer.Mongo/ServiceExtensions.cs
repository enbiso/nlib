using Duende.IdentityServer.Stores;
using Enbiso.NLib.IdentityServer.Mongo.Models;
using Enbiso.NLib.IdentityServer.Mongo.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Enbiso.NLib.IdentityServer.Mongo
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIdentityServerMongo<TUser, TRole>(this IServiceCollection services,
            Action<IdentityServerMongoOptions> optSetup)
            where TRole : IdentityRole, IMongoIdentityRole 
            where TUser : IdentityUser, IMongoIdentityUser
        {
            var options = new IdentityServerMongoOptions();
            optSetup.Invoke(options);
            var mongo = new MongoClient(options.ConnectionString);
            var db = mongo.GetDatabase(options.Database);
            services.AddSingleton(sp => db.GetCollection<ClientData>(options.ClientCollection));
            services.AddSingleton(sp => db.GetCollection<PersistedGrantData>(options.PersistedGrantCollection));
            services.AddSingleton(sp => db.GetCollection<ApiResourceData>(options.ApiResourceCollection));
            services.AddSingleton(sp => db.GetCollection<IdentityResourceData>(options.IdentityResourceCollection));
            services.AddSingleton(sp => db.GetCollection<TRole>(options.RoleCollection));
            services.AddSingleton(sp => db.GetCollection<TUser>(options.UserCollection));

            services.AddTransient<IClientStore, MongoClientStore>();
            services.AddTransient<IPersistedGrantStore, MongoPersistedGrantStore>();
            services.AddTransient<IResourceStore, MongoResourceStore>();
            services.AddTransient<IUserStore<TUser>, MongoUserStore<TUser>>();
            services.AddTransient<IRoleStore<TRole>, MongoRoleStore<TRole>>();
            
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