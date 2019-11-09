using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventService
    {
        Task PublishToBus<T>(T @event, string exchange = null) where T: IEvent;
    }
}
