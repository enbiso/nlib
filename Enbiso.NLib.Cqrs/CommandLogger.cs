using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.Cqrs
{
    public class LoggingBehavior<TCommand, TCommandResponse> : IPipelineBehavior<TCommand, TCommandResponse>
    {
        private readonly ILogger<LoggingBehavior<TCommand, TCommandResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TCommand, TCommandResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TCommandResponse> Handle(TCommand request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TCommandResponse> next)
        {
            _logger.LogInformation($"Handling {typeof(TCommand).Name}");
            var response = await next();
            _logger.LogInformation($"Handled {typeof(TCommandResponse).Name}");
            return response;
        }
    }
}