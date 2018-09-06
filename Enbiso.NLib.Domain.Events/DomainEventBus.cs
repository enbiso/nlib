using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Enbiso.NLib.Domain.Events
{
    /// <summary>
    /// Domain event bus
    /// </summary>
    public interface IDomainEventBus
    {
        Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default(CancellationToken)) where TDomainEvent : IDomainEvent;
        IEnumerable<Task> PublishFromEntities(IEnumerable<IEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Domain event bus implementation
    /// </summary>
    public class DomainEventBus: IDomainEventBus
    {
        private readonly Mediator _mediator;

        public DomainEventBus(Mediator mediator)
        {
            _mediator = mediator;
        }        

        public Task Publish<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default(CancellationToken)) where TDomainEvent : IDomainEvent
        {
            return _mediator.Publish(@event, cancellationToken);
        }

        public IEnumerable<Task> PublishFromEntities(IEnumerable<IEntity> entities,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return entities.GetEvents<IDomainEvent>().Select(async e => await Publish(e, cancellationToken)) ;
        }
    }
}
