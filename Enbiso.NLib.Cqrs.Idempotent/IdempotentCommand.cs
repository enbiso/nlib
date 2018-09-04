using System;
using System.Threading;
using System.Threading.Tasks;
using Enbiso.NLib.Idempotency;
using MediatR;

namespace Enbiso.NLib.Cqrs.Idempotent
{
    /// <summary>
    /// Idempotent Command
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class IdempotentCommand<TCommand, TResponse> : ICommand<TResponse> 
        where TCommand : ICommand<TResponse> 
        where TResponse : ICommandResponse
    {
        public IdempotentCommand(TCommand command, Guid id)
        {
            Command = command;
            Id = id;
        }

        public TCommand Command { get; }
        public Guid Id { get; }
    }

    /// <summary>
    /// Idempotent command handler
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class IdempotentCommandHandler<TCommand, TResponse> : ICommandHandler<IdempotentCommand<TCommand, TResponse>, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : ICommandResponse
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;

        public IdempotentCommandHandler(IMediator mediator, IRequestManager requestManager)
        {
            _mediator = mediator;
            _requestManager = requestManager;
        }

        public async Task<TResponse> Handle(IdempotentCommand<TCommand, TResponse> command,
            CancellationToken cancellationToken)
        {
            if (await _requestManager.ExistAsync(command.Id))
            {
                return await _mediator.Send(new GetDuplicateCommand<TCommand, TResponse>(command.Command), cancellationToken);
            }
            await _requestManager.CreateRequestForAsync<TCommand>(command.Id);
            return await _mediator.Send(command.Command, cancellationToken);
        }
    }
}