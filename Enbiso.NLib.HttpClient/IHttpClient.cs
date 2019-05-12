using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Enbiso.NLib.HttpClient
{
    /// <summary>
    /// Http Client
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="authorizationMethod"></param>
        /// <returns></returns>
        Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer");

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="requestId"></param>
        /// <param name="authorizationMethod"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> GetAsync(string uri, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");

        /// <summary>
        /// Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="item"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="requestId"></param>
        /// <param name="authorizationMethod"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="requestId"></param>
        /// <param name="authorizationMethod"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null,
            string authorizationMethod = "Bearer");

        /// <summary>
        /// Put
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="item"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="requestId"></param>
        /// <param name="authorizationMethod"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");
    }
}