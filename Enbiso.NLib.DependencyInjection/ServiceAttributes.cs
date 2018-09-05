using System;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.DependencyInjection
{
    public abstract class ServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }
        public Type[] ServiceTypes { get; }

        protected ServiceAttribute(ServiceLifetime lifetime, params Type[] serviceTypes)
        {
            Lifetime = lifetime;
            ServiceTypes = serviceTypes;
        }
    }

    /// <summary>
    /// Add as Transient ServiceTypes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TransientServiceAttribute : ServiceAttribute
    {
        public TransientServiceAttribute(params Type[] serviceTypes) : base(ServiceLifetime.Transient, serviceTypes)
        {
        }
    }

    /// <summary>
    /// Add as Scoped ServiceTypes
    /// </summary>

    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedServiceAttribute : ServiceAttribute
    {
        public ScopedServiceAttribute(params Type[] serviceTypes) : base(ServiceLifetime.Scoped, serviceTypes)
        {
        }
    }

    /// <summary>
    /// Add as Singleton ServiceTypes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonServiceAttribute : ServiceAttribute
    {
        public SingletonServiceAttribute(params Type[] serviceTypes) : base(ServiceLifetime.Singleton, serviceTypes)
        {
        }
    }
}
