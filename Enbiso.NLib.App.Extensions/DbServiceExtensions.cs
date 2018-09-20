using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.App.Extensions
{
    public static class DbServiceExtensions
    { 
        /// <summary>
        /// Add database
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="optionsLifetime"></param>
        /// <param name="assembly"></param>
        public static void AddDatabase<TContext>(this IServiceCollection services, string connectionString, 
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped, Assembly assembly = null)
            where TContext : DbContext
        {
            assembly = assembly ?? Assembly.GetCallingAssembly();
            services.AddDbContext<TContext>(
                options => { options.UseDatabase(connectionString, assembly); }, 
                contextLifetime, optionsLifetime);
        }

        /// <summary>
        /// Use Database for options
        /// </summary>
        /// <param name="options"></param>
        /// <param name="connectionString"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder options, string connectionString, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly();
            options.UseMySql(connectionString, builder =>
            {
                builder.MigrationsAssembly(assembly.GetName().Name);
                builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), new List<int>());
            });
            return options;
        }
    }
}