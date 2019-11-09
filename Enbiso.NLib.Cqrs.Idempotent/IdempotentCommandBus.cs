using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Enbiso.NLib.Idempotency;

namespace Enbiso.NLib.Cqrs.Idempotent
{
    /// <summary>
    /// Idempotent Command bus
    /// </summary>
    public interface IIdempotentCommandBus
    {
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command, string requestId,
            CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : ICommandResponse;
    }

    /// <inheritdoc />
    /// <summary>
    /// Idempotent Command bus implementation
    /// </summary>
    public class IdempotentCommandBus: IIdempotentCommandBus
    {
        private readonly ICommandBus _bus;
        private readonly IRequestManager _requestManager;

        public IdempotentCommandBus(ICommandBus bus, IRequestManager requestManager)
        {
            _bus = bus;
            _requestManager = requestManager;
        }

        public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, string requestId, CancellationToken cancellationToken = default(CancellationToken))            
            where TResponse : ICommandResponse
        {
            if (!Guid.TryParse(requestId, out var reqGuid))            
                throw new InvalidRequestIdException(requestId);

            var reqLog = await _requestManager.FindAsync(reqGuid);
            if (reqLog != null)
            {
                return JsonSerializer.Deserialize<TResponse>(reqLog.Response);
            }
            var response = await _bus.Send(command, cancellationToken);
            await _requestManager.CreateRequestAsync(reqGuid, command.GetType().Name, JsonSerializer.Serialize(response));
            return response;
        }
    }

    /// <summary>
    /// Invalid request ID exception
    /// </summary>
    public class InvalidRequestIdException : Exception
    {
        public string RequestId { get; private set; }
        public InvalidRequestIdException(string requestId) : base($"Error parsing request ID")
        {
            RequestId = requestId;
        }
    }
}
