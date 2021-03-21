using System.Threading.Tasks;

namespace Enbiso.NLib.EventBus.AwsSns
{
    public class AwsSnsEventSubscriber: IEventSubscriber
    {
        public void Dispose()
        {
        }

        public Task Subscribe()
        {
            return Task.CompletedTask;
        }
    }
}