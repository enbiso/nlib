using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Enbiso.NLib.Cqrs
{
    public interface ICommandBus
    {
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command,
            CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : ICommandResponse;
    }

    public class CommandBus : ICommandBus
    {
        private readonly IMediator _mediator;

        public CommandBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : ICommandResponse
        {
            command = command ?? throw new CommandValidationException(typeof(ICommand<TResponse>));
            return _mediator.Send(command, cancellationToken);
        }
    }
}
