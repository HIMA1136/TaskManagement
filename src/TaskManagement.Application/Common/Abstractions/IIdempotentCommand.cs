namespace TaskManagement.Application.Common.Abstractions;

public interface IIdempotentCommand<TResponse> : ICommand<TResponse>
{
    Guid IdempotencyKey { get; }
}
