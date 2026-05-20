using MediatR;
using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Application.Common.Behaviors;

public sealed class IdempotencyBehavior<TRequest, TResponse>(
    IIdempotencyService idempotencyService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var existing = await idempotencyService.GetAsync<TResponse>(request.IdempotencyKey, cancellationToken);
        if (existing is not null) return existing;

        var response = await next();

        await idempotencyService.StoreAsync(request.IdempotencyKey, typeof(TRequest).Name, response!, cancellationToken);
        return response;
    }
}
