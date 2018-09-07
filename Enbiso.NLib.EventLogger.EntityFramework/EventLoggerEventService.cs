using System.Threading.Tasks;
using Enbiso.NLib.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enbiso.NLib.EventLogger.EntityFramework
{
    public class EntityEventLoggerEventService<TDbContext>: EventLoggerEventService where TDbContext: DbContext
    {
        private readonly TDbContext _dbContext;

        public EntityEventLoggerEventService(IEventBus bus, IEventLoggerService service, TDbContext dbContext) : base(bus, service)
        {
            _dbContext = dbContext;
        }

        protected override async Task PrePublish(IEvent @event)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    await _dbContext.SaveChangesAsync();
                    await Service.SaveEventAsync(@event, _dbContext.Database.CurrentTransaction.GetDbTransaction());
                    transaction.Commit();
                }
            });
        }
    }
}