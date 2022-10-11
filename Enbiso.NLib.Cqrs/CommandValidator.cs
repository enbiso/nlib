using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Enbiso.NLib.Cqrs
{
    public class ValidatorBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : ICommandResponse
    {
        private readonly IEnumerable<ICommandValidator<TCommand>> _validators;

        public ValidatorBehavior(IEnumerable<ICommandValidator<TCommand>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TCommand request,
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validatorTasks = _validators.Select(v => v.Validate(request)).ToArray();
            await Task.WhenAll(validatorTasks);
            var failures = validatorTasks.SelectMany(t => t.Result).Where(error => error != null).ToArray();
            
            if (failures.Any())                                    
                throw new CommandValidationException(typeof(TCommand), failures);

            var response = await next();
            return response;
        }
    }

    /// <summary>
    /// Command validator
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>    
    public interface ICommandValidator<TCommand> 
        where TCommand: IBaseCommand        
    {
        Task<IEnumerable<ValidationError>> Validate(TCommand command);
    }
    
    /// <summary>
    /// Validation ValidationError
    /// </summary>
    public class ValidationError
    {
        public string Message { get; }
        public string Property { get; }

        public ValidationError(string message, string property)
        {
            Message = message;
            Property = property;
        }
    }

    /// <summary>
    /// Command Validation exception
    /// </summary>
    public class CommandValidationException : Exception
    {
        public IEnumerable<ValidationError> Errors { get; }
        public CommandValidationException(Type command, IEnumerable<ValidationError> failures = null)
            : this($"{command.Name} validation failed")
        {
            Errors = failures;
        }
        public CommandValidationException(string message): base(message) {

        }
    }
}