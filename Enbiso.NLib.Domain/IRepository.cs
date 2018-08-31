using System;
using System.Threading;
using System.Threading.Tasks;
using Enbiso.NLib.Domain.Models;

namespace Enbiso.NLib.Domain.Repository
{
    /// <summary>
    /// Repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IRootEntity
    {
        IUnitOfWork UnitOfWork { get; }
    }

    /// <summary>
    /// Unit of work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}