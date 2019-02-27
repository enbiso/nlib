using System;
using System.Collections.Generic;
using System.Linq;

namespace Enbiso.NLib.Domain
{
    /// <summary>
    /// Aggregated root entity
    /// </summary>
    public interface IRootEntity
    {
    }

    /// <summary>
    /// Base Event
    /// </summary>
    public interface IEntityEvent
    {
    }

    /// <summary>
    /// Domain Entity interface
    /// </summary>
    public interface IEntity
    {
        List<IEntityEvent> GetEvents();
    }

    public abstract class Entity: IEntity
    {
        private readonly List<IEntityEvent> _events = new List<IEntityEvent>();
        
        public void AddEvent(IEntityEvent @event)
        {            
            _events.Add(@event?? throw new ArgumentNullException());
        }
        
        public void RemoveEvent(IEntityEvent @event)
        {            
            _events.Remove(@event ?? throw new ArgumentNullException());
        }

        public List<IEntityEvent> GetEvents() => _events;
    }

    /// <inheritdoc />
    /// <summary>
    /// Domain Entity abstraction
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Entity<TKey>: Entity
    {
        private int? _requestedHashCode;
        public virtual TKey Id { get; set; }

        public bool IsTransient()
        {
            return default(TKey)?.Equals(Id) ?? true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity<TKey>))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            var item = (Entity<TKey>)obj;

            if (item.IsTransient() || IsTransient())
                return false;

            return item.Id?.Equals(Id) ?? false;
        }

        public override int GetHashCode()
        {
            if (IsTransient()) return base.GetHashCode();

            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            return left?.Equals(right) ?? Equals(right, null);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Entity helper extensions
    /// </summary>
    public static class EntityExtensions
    {
        public static void ClearEvents(this IEnumerable<IEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.GetEvents().Clear();
            }
        }

        public static IEnumerable<TEvent> GetEvents<TEvent>(this IEnumerable<IEntity> entities) where TEvent: IEntityEvent
        {
            return entities.ToList().SelectMany(e => e.GetEvents()).OfType<TEvent>();
        }
    }
}
