using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.Domain
{
    /// <inheritdoc />
    /// <summary>
    /// Repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IRepository where T : IRootEntity
    {        
    }

    /// <summary>
    /// Repository
    /// </summary>    
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Unit of work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}