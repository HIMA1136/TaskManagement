using MediatR;
using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.Application.Common.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }

public interface ICommand : IRequest<Result> { }

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse> { }

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand { }
