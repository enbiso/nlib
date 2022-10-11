using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.Cqrs
{
    public class LoggingBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : ICommandResponse
    {
        private readonly ILogger<LoggingBehavior<TCommand, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TCommand, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogTrace("Handling {Name}", typeof(TCommand).Name);
                var response = await next();
                _logger.LogTrace("Handled {Name}", typeof(TCommand).Name);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed {Name}", typeof(TResponse).Name);
                throw;
            }
        }
    }
}