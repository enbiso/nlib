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

        public Task<bool> ExistAsync(Guid id)
        {
            return _repo.ExistsAsync(id);
        }

        public async Task CreateRequestForAsync<T>(Guid id)
        {
            var exists = await _repo.ExistsAsync(id);

            var request = exists ?
                throw new Exception($"Request with {id} already exists") : 
                new RequestLog
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _repo.Add(request);

            await _repo.SaveChangesAsync();
        }
    }
}