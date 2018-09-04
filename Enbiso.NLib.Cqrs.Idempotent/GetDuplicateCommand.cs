namespace Enbiso.NLib.Cqrs.Idempotent
{
    /// <summary>
    /// Get duplicate response command
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class GetDuplicateCommand<TCommand, TResponse> : ICommand<TResponse> 
        where TCommand: ICommand<TResponse>
        where TResponse : ICommandResponse
    {
        public GetDuplicateCommand(TCommand command)
        {
            Command = command;
        }

        public TCommand Command { get; }
    }

    /// <summary>
    /// Get response for duplicate command handler
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IGetDuplicateCommandHandler<TCommand, TResponse> : ICommandHandler<GetDuplicateCommand<TCommand, TResponse>, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : ICommandResponse
    {
        
    }
}
