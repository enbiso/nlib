using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus
{
    public interface IEventService
    {
        Task PublishToBus(IEvent @event);
    }
}
