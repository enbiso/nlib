using MediatR;

namespace Enbiso.NLib.Cqrs
{
    public interface ICommand<out TResponse>: IRequest<TResponse> where TResponse: ICommandResponse
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
