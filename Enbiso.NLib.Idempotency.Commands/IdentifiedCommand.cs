using System;
using MediatR;

namespace Enbiso.NLib.EventLogger.Commands
{
    public class IdentifiedCommand<TCommand, TRequest> : IRequest<TRequest> where TCommand : IRequest<TRequest>
    {
        public IdentifiedCommand(TCommand command, Guid id)
        {
            Command = command;
            Id = id;
        }

        public TCommand Command { get; }
        public Guid Id { get; }
    }
}