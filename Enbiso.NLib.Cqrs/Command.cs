using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;

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

    public interface ICommandPreProcessor<in TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse: ICommandResponse
    {
        Task Process(TCommand request, CancellationToken cancellationToken);
    }

    public interface ICommandPostProcessor<in TCommand, in TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : ICommandResponse
    {
        Task Process(TCommand request, TResponse response, CancellationToken cancellationToken);
    }
}
