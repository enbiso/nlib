namespace Enbiso.NLib.Domain.Events
{
    /// <summary>
    /// Domain Event
    /// </summary>
    public interface IDomainEvent
    {
    }

    /// <summary>
    /// Domain Event handler
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {

    }
}