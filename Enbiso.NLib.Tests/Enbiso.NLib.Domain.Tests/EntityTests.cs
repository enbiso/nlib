using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Enbiso.NLib.Domain.Events;
using Enbiso.NLib.Domain.Models;
using Xunit;

namespace Enbiso.NLib.Domain.Tests
{
    public class EntityTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        [Fact]        
        public void IsTransient_NoNull_False()
        {            
            var entity = _fixture.Create<Entity<Guid>>();
            entity.Id = Guid.NewGuid();
            Assert.False(entity.IsTransient());
        }

        [Fact]        
        public void IsTransient_Empty_True()
        {
            var entity = _fixture.Create<Entity<Guid>>();
            Assert.True(entity.IsTransient());
        }
        
        [Theory]                
        [InlineData(0, false)]
        [InlineData(3, true)]
        public void Equals_NotNull_Result(int id, bool result)
        {            
            var e1 = _fixture.Create<Entity<int>>();
            var e2 = _fixture.Create<Entity<int>>();
            e1.Id = id;
            e2.Id = id;
            Assert.True(e1 == e2 == result);
        }

        [Fact]        
        public void AddDomainEvent_ListOfIDomainEvent_Success()
        {            
            var entity = _fixture.Create<Entity<Guid>>();
            var events = _fixture.Create<List<IDomainEvent>>();
            entity.DomainEvents.Clear();

            events.ForEach(entity.AddDomainEvent);
            Assert.Equal(entity.DomainEvents.Count, events.Count);
        }
        
        [Fact]        
        public void RemoveDomainEvent_ValidEvent_Success()
        {            
            var entity = _fixture.Create<Entity<Guid>>();
            var events = _fixture.Create<List<IDomainEvent>>();            

            events.ForEach(entity.AddDomainEvent);
            entity.RemoveDomainEvent(events.First());
            Assert.Equal(entity.DomainEvents.Count, events.Count - 1);
        }
        
                
        [Fact]                
        public void RemoveDomainEvent_Null_Success()
        {
            var events = _fixture.Create<List<IDomainEvent>>();
            var entity = _fixture.Create<Entity<Guid>>();            
            events.ForEach(entity.AddDomainEvent);            
            Assert.Throws<ArgumentNullException>(() => entity.RemoveDomainEvent(null));  
        }
        
        [Fact]        
        public void AddDomainEvent_Nulls_Success()
        {            
            var entity = _fixture.Create<Entity<Guid>>();
            Assert.Throws<ArgumentNullException>(() => entity.AddDomainEvent(null));            
        }
    }
}
