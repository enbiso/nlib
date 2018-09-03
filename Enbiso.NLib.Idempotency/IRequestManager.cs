using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.Idempotency
{
    /// <summary>
    /// Request Manager Interface
    /// </summary>
    public interface IRequestManager
    {
        /// <summary>
        /// Check exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(Guid id);

        /// <summary>
        /// Create request for command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task CreateRequestForAsync<T>(Guid id);
    }
}