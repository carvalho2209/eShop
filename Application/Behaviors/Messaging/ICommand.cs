using MediatR;

namespace Application.Behaviors.Messaging;

public interface ICommand : IRequest, ICommandBase
{
    
}

public interface ICommandBase
{
}

public interface ICommand<TResponse> : IRequest<TResponse>, ICommandBase
{
    
}
