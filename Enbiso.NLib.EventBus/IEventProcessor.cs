using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string eventName, byte[] data);
        void Setup(Action<string> onAddSubscription = null);
    }
}