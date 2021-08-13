using System;
using Amazon;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.AwsConnection
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void AddAwsConnection(this IServiceCollection services, Action<AwsConnectionOptions> option)
        {
            services.Configure(option);
            var opts = new AwsConnectionOptions();
            option.Invoke(opts);
            services.AddSingleton<AWSCredentials>(new BasicAWSCredentials(opts.AccessKey, opts.SecretKey));
            services.AddSingleton(RegionEndpoint.GetBySystemName(opts.Region));
        }
    }
}