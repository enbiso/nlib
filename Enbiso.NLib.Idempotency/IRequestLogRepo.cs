using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.Idempotency
{
    /// <summary>
    /// Request log repository
    /// </summary>
    public interface IRequestLogRepo
    {
        /// <summary>
        /// Find exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Add New
        /// </summary>
        /// <param name="requestLog"></param>
        /// <returns></returns>
        RequestLog Add(RequestLog requestLog);

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}