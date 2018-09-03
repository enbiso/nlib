using System.Threading;
using System.Threading.Tasks;
using Enbiso.NLib.Idempotency;
using MediatR;

namespace Enbiso.NLib.EventLogger.Commands
{
    public class
        IdentifiedCommandHandler<TCommand, TResponse> : IRequestHandler<IdentifiedCommand<TCommand, TResponse>,
            TResponse>
        where TCommand : IRequest<TResponse>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;

        public IdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager)
        {
            _mediator = mediator;
            _requestManager = requestManager;
        }

        public async Task<TResponse> Handle(IdentifiedCommand<TCommand, TResponse> request,
            CancellationToken cancellationToken)
        {
            var alreadyExists = await _requestManager.ExistAsync(request.Id);
            if (alreadyExists)
                return await CreateResultForDuplicateRequest(request.Command);
            await _requestManager.CreateRequestForAsync<TCommand>(request.Id);
            return await _mediator.Send(request.Command, cancellationToken);
        }

        protected virtual Task<TResponse> CreateResultForDuplicateRequest(TCommand request)
        {
            return Task.FromResult(default(TResponse));
        }
    }
}