using System;

namespace Enbiso.NLib.EventBus
{

    /// <summary>
    /// Subscription details
    /// </summary>
    public class SubscriptionInfo : ISubscriptionInfo
    {
        /// <summary>
        /// Is subscription dynamic
        /// </summary>
        public bool IsDynamic { get; }

        /// <summary>
        /// Subscription handler type
        /// </summary>
        public Type HandlerType { get; }

        private SubscriptionInfo(bool isDynamic, Type handlerType)
        {
            IsDynamic = isDynamic;
            HandlerType = handlerType;
        }

        /// <summary>
        /// Create dynamic subscription
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static SubscriptionInfo Dynamic(Type handlerType)
        {
            return new SubscriptionInfo(true, handlerType);
        }

        /// <summary>
        /// Create strict type subscription
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(false, handlerType);
        }
    }
}
