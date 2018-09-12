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
        /// Find Async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RequestLog> FindAsync(Guid id);

        /// <summary>
        /// Create request for command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task CreateRequestForAsync<T>(Guid id, string response);

        /// <summary>
        /// Create Request Log by name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task CreateRequestAsync(Guid id, string name, string response);
    }
}