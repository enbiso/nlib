using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Enbiso.NLib.EventLogger.Mongo
{
    public static class ServiceExtensions
    {
        public static void AddEventLogger(this IServiceCollection services, string dbname)
        {
            services.AddEventLogger();
            services.AddTransient<IEventLogRepo>(sp => new MongoEventLogRepo(sp.GetService<IMongoClient>(), dbname));
        }
    }
}