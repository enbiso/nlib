using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Enbiso.NLib.App.Extensions
{
    public static class DbServiceExtensions
    {
        public static void AddDatabase<TContext>(this IServiceCollection services, string connectionString)
            where TContext : DbContext
        {
            var assembly = Assembly.GetCallingAssembly();
            services.AddDatabase<TContext>(connectionString, assembly);
        }

        public static void AddDatabase<TContext>(this IServiceCollection services, string connectionString, Assembly assembly)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(options => { options.UseDatabase(connectionString, assembly); });
        }

        private static void UseDatabase(this DbContextOptionsBuilder options, string connectionString, Assembly assembly)
        {
            options.UseMySql(connectionString, builder =>
            {
                builder.MigrationsAssembly(assembly.GetName().Name);
                builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
            });
        }
    }
}