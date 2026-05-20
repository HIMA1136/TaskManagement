using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Application.Common.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICachedQueryMarker cachedQuery)
            return await next();

        var cacheKey = cachedQuery.GetCacheKey();
        var expiration = cachedQuery.GetExpiration();

        var cached = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cached;
        }

        var response = await next();

        await cacheService.SetAsync(cacheKey, response, expiration, cancellationToken);
        logger.LogDebug("Cache miss, stored {CacheKey}", cacheKey);

        return response;
    }
}
