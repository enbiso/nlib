using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Enbiso.NLib.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Enbiso.NLib.App.Extensions
{
    public static class DbServiceExtensions
    {
        public static void AddDatabase<TContext>(this IServiceCollection services, IAppSettings settings)
            where TContext : DbContext
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => !a.IsDynamic);
            services.AddDatabase<TContext>(settings, assembly);
        }

        public static void AddDatabase<TContext>(this IServiceCollection services, IAppSettings settings, Assembly assembly)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(options => { options.UseDatabase(settings, assembly); });
            services.AddTransient<IDbConnection>(ctx => new MySqlConnection(settings.ConnectionString));
        }

        public static void UseDatabase(this DbContextOptionsBuilder options, IAppSettings settings, Assembly assembly)
        {
            options.UseMySql(settings.ConnectionString, builder =>
            {
                builder.MigrationsAssembly(assembly.GetName().Name);
                builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
            });
        }
    }
}