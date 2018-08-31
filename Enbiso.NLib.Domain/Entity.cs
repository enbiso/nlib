using System;
using System.Collections.Generic;
using Enbiso.NLib.Domain.Events;

namespace Enbiso.NLib.Domain.Models
{
    /// <summary>
    /// Agregated root entity
    /// </summary>
    public interface IRootEntity
    {
    }

    /// <summary>
    /// Domain Entity interface
    /// </summary>
    public interface IEntity
    {
        List<IDomainEvent> DomainEvents { get; set; }
    }

    /// <summary>
    /// Domain Entity abstraction
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Entity<TKey>: IEntity
    {
        private int? _requestedHashCode;
        public virtual TKey Id { get; set; }


        public List<IDomainEvent> DomainEvents { get; set; } = new List<IDomainEvent>();

        public void AddDomainEvent(IDomainEvent eventItem)
        {            
            DomainEvents.Add(eventItem?? throw new ArgumentNullException());
        }
        
        public void RemoveDomainEvent(IDomainEvent eventItem)
        {            
            DomainEvents.Remove(eventItem?? throw new ArgumentNullException());
        }

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
            return Equals(left, null) ? Equals(right, null) : left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}
