using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.Idempotency
{
    /// <inheritdoc />
    /// <summary>
    /// Request manager implementation
    /// </summary>
    public class RequestManager : IRequestManager
    {
        private readonly IRequestLogRepo _repo;

        public RequestManager(IRequestLogRepo repo)
        {
            _repo = repo;
        }

        public Task<RequestLog> FindAsync(Guid id)
        {
            return _repo.FindAsync(id);
        }

        public Task CreateRequestForAsync<T>(Guid id, string response)
        {
            return CreateRequestAsync(id, typeof(T).Name, response);
        }

        public async Task CreateRequestAsync(Guid id, string name, string response)
        {            
            var request = new RequestLog
            {
                Id = id,
                Name = name,
                Time = DateTime.UtcNow,
                Response = response
            };
            _repo.Add(request);
            await _repo.SaveChangesAsync();
        }    
    }
}