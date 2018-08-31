using System.Collections.Generic;

namespace Enbiso.NLib.EventBus.Abstractions
{
    public interface IEventBusSubscriber
    {
        void Subscribe();
    }

    public static class EventBusSubscriberExtensions
    {
        public static void Subscribe(this IEnumerable<IEventBusSubscriber> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Subscribe();
            }
        }
    }
}