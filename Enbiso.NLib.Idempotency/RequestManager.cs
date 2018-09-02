using System;
using System.Threading.Tasks;

namespace Enbiso.NLib.Idempotency
{
    public class RequestManager : IRequestManager
    {
        private readonly IRequestLogRepo _repo;

        public RequestManager(IRequestLogRepo repo)
        {
            _repo = repo;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
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