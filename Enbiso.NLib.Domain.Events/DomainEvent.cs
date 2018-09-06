using MediatR;

namespace Enbiso.NLib.Domain.Events
{
    /// <summary>
    /// Domain event
    /// </summary>
    public interface IDomainEvent: INotification, IEntityEvent
    {
    }

    /// <summary>
    /// Domain event handlers
    /// </summary>
    /// <typeparam name="TDomainEvent"></typeparam>
    public interface IDomainEventHandler<in TDomainEvent>: INotificationHandler<TDomainEvent> 
        where TDomainEvent: IDomainEvent        
    {

    }
}
