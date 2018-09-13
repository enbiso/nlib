using MediatR;

namespace Enbiso.NLib.Cqrs
{
    public interface IBaseCommand
    {
    }

    public interface ICommand<out TResponse>: IRequest<TResponse>, IBaseCommand where TResponse: ICommandResponse
    {
    }

    public interface ICommandResponse
    {
        
    }

    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
        where TCommand : ICommand<TResponse> 
        where TResponse: ICommandResponse
    {
        
    }
}
