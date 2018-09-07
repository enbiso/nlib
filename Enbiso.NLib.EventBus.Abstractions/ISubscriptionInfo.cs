using System;

namespace Enbiso.NLib.EventBus
{
    public interface ISubscriptionInfo
    {
        bool IsDynamic { get; }
        Type HandlerType { get; }
    }
}
