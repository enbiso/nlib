using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enbiso.NLib.Cqrs.Idempotent
{
    /// <summary>
    /// Idempotent Command bus
    /// </summary>
    public interface IIdempotentCommandBus
    {
        Task<TResponse> Send<TCommand, TResponse>(TCommand command, string requestId, CancellationToken cancellationToken = default(CancellationToken))
            where TCommand: ICommand<TResponse>
            where TResponse : ICommandResponse;
    }

    /// <inheritdoc />
    /// <summary>
    /// Idempotent Command bus implementation
    /// </summary>
    public class IdempotentCommandBus: IIdempotentCommandBus
    {
        private readonly ICommandBus _bus;

        public IdempotentCommandBus(ICommandBus bus)
        {
            _bus = bus;
        }

        public Task<TResponse> Send<TCommand, TResponse>(TCommand command, string requestId, CancellationToken cancellationToken = default(CancellationToken))
            where TCommand : ICommand<TResponse>
            where TResponse : ICommandResponse
        {
            if (!Guid.TryParse(requestId, out var requestGuId))            
                throw new InvalidRequestIdException(requestId);            

            return _bus.Send(new IdempotentCommand<TCommand, TResponse>(command, requestGuId), cancellationToken);
        }
    }

    /// <summary>
    /// Invalid request ID exception
    /// </summary>
    public class InvalidRequestIdException : Exception
    {
        public InvalidRequestIdException(string requestId) : base($"Error parsing request id {requestId}")
        {
        }
    }
}
