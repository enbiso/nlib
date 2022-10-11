using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Enbiso.NLib.Cqrs
{
    public class ProcessorBehaviour<TCommand, TResponse>: IPipelineBehavior<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : ICommandResponse
    {
        private readonly IEnumerable<ICommandPreProcessor<TCommand, TResponse>> _preProcessors;
        private readonly IEnumerable<ICommandPostProcessor<TCommand, TResponse>> _postProcessors;

        public ProcessorBehaviour(IEnumerable<ICommandPreProcessor<TCommand, TResponse>> preProcessors,
            IEnumerable<ICommandPostProcessor<TCommand, TResponse>> postProcessors)
        {
            _preProcessors = preProcessors;
            _postProcessors = postProcessors;
        }

        public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                _preProcessors.Select(p => p.Process(request, cancellationToken)));
            
            var response = await next();

            await Task.WhenAll(
                _postProcessors.Select(p => p.Process(request, response, cancellationToken)));

            return response;
        }
    }
}