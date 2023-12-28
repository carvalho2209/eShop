using MediatR;

namespace Application.Abstractions.Behaviors.Messaging;

public interface ICommand : IRequest, ICommandBase
{
    
}

public interface ICommandBase
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>, ICommandBase
{
    
}
